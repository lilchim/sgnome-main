using SteamApi.Models.Steam.Responses;
using SteamApi.Models.Steam.Store;

namespace Sgnome.Clients.Steam;

/// <summary>
/// Interface for Steam API operations with centralized caching.
/// </summary>
public interface ISteamClient
{
    /// <summary>
    /// Gets owned games for a Steam user with basic information.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    Task<T> GetOwnedGamesAsync<T>(string steamId, Func<OwnedGamesResponse, T> transform);

    /// <summary>
    /// Gets owned games for a Steam user with detailed game information.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    Task<T> GetOwnedGamesWithDetailsAsync<T>(string steamId, Func<OwnedGamesResponse, T> transform);

    /// <summary>
    /// Gets recently played games for a Steam user.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    Task<T> GetRecentlyPlayedGamesAsync<T>(string steamId, Func<OwnedGamesResponse, T> transform);

    /// <summary>
    /// Gets player details for a Steam user.
    /// </summary>
    /// <param name="steamId">The Steam ID of the user</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    Task<T> GetPlayerDetailsAsync<T>(string steamId, Func<PlayerSummariesResponse, T> transform);

    /// <summary>
    /// Gets app details for a Steam app.
    /// </summary>
    /// <param name="appId">The Steam app ID</param>
    /// <param name="transform">Function to transform the response</param>
    /// <returns>Transformed result from the Steam API</returns>
    Task<T> GetAppDetailsAsync<T>(string appId, Func<Dictionary<string, StoreAppDetailsResponse>, T> transform);
} 