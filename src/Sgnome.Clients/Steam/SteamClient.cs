using SteamApi.Client;
using SteamApi.Models.Steam.Responses;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using SteamApi.Models.Steam.Store;

namespace Sgnome.Clients.Steam;

/// <summary>
/// Client for Steam API operations with centralized caching.
/// Exposes high-level methods for domain services to use.
/// </summary>
public class SteamClient : ISteamClient
{
    private readonly ISteamApiClient _apiClient;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<SteamClient> _logger;

    public SteamClient(
        ISteamApiClient apiClient,
        IConnectionMultiplexer redis,
        ILogger<SteamClient> logger)
    {
        _apiClient = apiClient;
        _redis = redis;
        _logger = logger;
    }

    /// <summary>
    /// Gets owned games for a Steam user with basic information.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    public async Task<T> GetOwnedGamesAsync<T>(string steamId, Func<OwnedGamesResponse, T> transform)
    {
        var cacheKey = $"steam:owned-games:{steamId}";
        return await ExecuteWithCacheAsync<SteamResponse<OwnedGamesResponse>, T>(cacheKey, 
            () => _apiClient.GetOwnedGamesAsync(steamId, true, true), 
            response => response?.Response != null ? transform(response.Response) : transform(null));
    }

    /// <summary>
    /// Gets owned games for a Steam user with detailed game information.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    public async Task<T> GetOwnedGamesWithDetailsAsync<T>(string steamId, Func<OwnedGamesResponse, T> transform)
    {
        var cacheKey = $"steam:owned-games-with-details:{steamId}";
        return await ExecuteWithCacheAsync<SteamResponse<OwnedGamesResponse>, T>(cacheKey,
            () => _apiClient.GetOwnedGamesAsync(steamId, includeAppInfo: true), 
            response => response?.Response != null ? transform(response.Response) : transform(null));
    }

    /// <summary>
    /// Gets recently played games for a Steam user.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    public async Task<T> GetRecentlyPlayedGamesAsync<T>(string steamId, Func<RecentlyPlayedGamesResponse, T> transform)
    {
        var cacheKey = $"steam:recently-played:{steamId}";
        return await ExecuteWithCacheAsync<SteamResponse<RecentlyPlayedGamesResponse>, T>(cacheKey,
            () => _apiClient.GetRecentlyPlayedGamesAsync(steamId), 
            response => response?.Response != null ? transform(response.Response) : transform(null));
    }

    /// <summary>
    /// Gets player details for a Steam user.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    public async Task<T> GetPlayerDetailsAsync<T>(string steamId, Func<PlayerSummariesResponse, T> transform)
    {
        var cacheKey = $"steam:player-summary:{steamId}";
        return await ExecuteWithCacheAsync<SteamResponse<PlayerSummariesResponse>, T>(cacheKey,
            () => _apiClient.GetPlayerSummariesAsync(steamId), 
            response => response?.Response != null ? transform(response.Response) : transform(null));
    }

    /// <summary>
    /// Gets app details for a Steam app.
    /// </summary>
    /// <param name="appId">The Steam app ID</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    public async Task<T> GetAppDetailsAsync<T>(string appId, Func<Dictionary<string, StoreAppDetailsResponse>, T> transform)
    {
        var cacheKey = $"steam:app-details:{appId}";
        return await ExecuteWithCacheAsync<Dictionary<string, StoreAppDetailsResponse>, T>(cacheKey,
            () => _apiClient.GetStoreAppDetailsAsync(int.Parse(appId)),
            response => response?.FirstOrDefault().Value != null ? transform(response) : transform(null));
    }

    /// <summary>
    /// Executes an API call with Redis caching.
    /// </summary>
    /// <typeparam name="T">The type of the API response</typeparam>
    /// <typeparam name="TResult">The type of the transformed result</typeparam>
    /// <param name="cacheKey">Redis cache key</param>
    /// <param name="apiCall">Function to make the API call</param>
    /// <param name="transform">Function to transform the API response</param>
    /// <param name="expiry">Cache expiry time (default: 30 minutes)</param>
    /// <returns>Transformed result from cache or API</returns>
    private async Task<TResult> ExecuteWithCacheAsync<T, TResult>(
        string cacheKey,
        Func<Task<T>> apiCall,
        Func<T, TResult> transform,
        TimeSpan? expiry = null)
    {
        try
        {
            var database = _redis.GetDatabase();
            expiry ??= TimeSpan.FromMinutes(30);

            // Try to get from cache first
            var cachedJson = await database.StringGetAsync(cacheKey);
            if (!cachedJson.IsNull)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                var cached = JsonSerializer.Deserialize<T>(cachedJson!);
                if (cached != null)
                {
                    return transform(cached);
                }
            }

            // Make API call
            _logger.LogDebug("Cache miss for key: {CacheKey}, making API call", cacheKey);
            var response = await apiCall();

            if (response != null)
            {
                // Cache the response
                var responseJson = JsonSerializer.Serialize(response);
                await database.StringSetAsync(cacheKey, responseJson, expiry);
                _logger.LogDebug("Cached response for key: {CacheKey}", cacheKey);

                return transform(response);
            }

            _logger.LogWarning("API call returned null for key: {CacheKey}", cacheKey);
            return transform(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing cached API call for key: {CacheKey}", cacheKey);
            throw;
        }
    }
} 