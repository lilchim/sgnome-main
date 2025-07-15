using SteamApi.Client;
using SteamApi.Models.Steam.Responses;
using Microsoft.Extensions.Logging;

namespace UserLibraryService.Providers;

public class SteamUserLibraryProvider : ISteamUserLibraryProvider
{
    private readonly ISteamApiClient _steamApiClient;
    private readonly ILogger<SteamUserLibraryProvider> _logger;

    public SteamUserLibraryProvider(ISteamApiClient steamApiClient, ILogger<SteamUserLibraryProvider> logger)
    {
        _steamApiClient = steamApiClient;
        _logger = logger;
    }

    public async Task<SteamResponse<OwnedGamesResponse>?> GetUserLibraryAsync(string steamId)
    {
        try
        {
            _logger.LogInformation("Fetching Steam library for user {SteamId}", steamId);

            var response = await _steamApiClient.GetOwnedGamesAsync(steamId);
            if (response?.Response == null)
            {
                _logger.LogWarning("No library data found for Steam ID {SteamId}", steamId);
                return null;
            }

            _logger.LogInformation("Successfully fetched {GameCount} games for Steam ID {SteamId}",
                response.Response.Games?.Count ?? 0, steamId);
            _logger.LogInformation("GameCount: {GameCount}", response.Response.GameCount);

            // Debug: Log first few games to see what we're getting
            if (response.Response.Games != null)
            {
                foreach (var game in response.Response.Games.Take(3))
                {
                    _logger.LogDebug("Game: AppId={AppId}, Name='{Name}', Playtime={Playtime}",
                        game.AppId, game.Name, game.PlaytimeForever);
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Steam library for user {SteamId}", steamId);
            return null;
        }
    }

    public async Task<SteamResponse<RecentlyPlayedGamesResponse>?> GetRecentlyPlayedGamesAsync(string steamId)
    {
        try
        {
            _logger.LogInformation("Fetching recently played games for user {SteamId}", steamId);

            var response = await _steamApiClient.GetRecentlyPlayedGamesAsync(steamId);
            if (response?.Response == null)
            {
                _logger.LogWarning("No recently played games found for Steam ID {SteamId}", steamId);
                return null;
            }

            _logger.LogInformation("Successfully fetched {GameCount} recently played games for Steam ID {SteamId}",
                response.Response.Games?.Count ?? 0, steamId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recently played games for user {SteamId}", steamId);
            return null;
        }
    }
}