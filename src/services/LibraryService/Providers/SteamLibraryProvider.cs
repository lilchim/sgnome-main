using SteamApi.Models.Steam.Responses;
using SteamApi.Client;
using Microsoft.Extensions.Logging;
using Sgnome.Models.Graph;

namespace LibraryService.Providers;

public class SteamLibraryProvider : ISteamLibraryProvider
{
    private readonly ISteamApiClient _steamApiClient;
    private readonly ILogger<SteamLibraryProvider> _logger;

    public SteamLibraryProvider(
        ISteamApiClient steamApiClient,
        ILogger<SteamLibraryProvider> logger)
    {
        _steamApiClient = steamApiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(string steamId)
    {
        try
        {
            var steamResponse = await _steamApiClient.GetOwnedGamesAsync(steamId);
            if (steamResponse?.Response == null)
            {
                _logger.LogWarning("No Steam library response for {SteamId}", steamId);
                return Enumerable.Empty<Pin>();
            }

            var ownedGames = steamResponse.Response;
            
            // Create the main Steam library pin
            var steamLibraryPin = new Pin
            {
                Id = $"steam-library-{steamId}",
                Label = $"Steam Library ({ownedGames.GameCount} games)",
                Type = "steam-library",
                Behavior = PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = $"{ownedGames.GameCount} games on Steam",
                    Count = ownedGames.GameCount,
                    Icon = "steam"
                },
                Metadata = new PinMetadata
                {
                    TargetNodeType = "organized-library",
                    OriginNodeId = $"player-{steamId}",
                    ApiEndpoint = "/api/organized-library/select",
                    Parameters = new Dictionary<string, object>
                    {
                        ["playerId"] = steamId,
                        ["librarySource"] = "steam"
                    }
                }
            };

            _logger.LogInformation("Retrieved Steam library data for {SteamId} with {GameCount} games", 
                steamId, ownedGames.GameCount);

            return new[] { steamLibraryPin };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Steam library data for {SteamId}", steamId);
            return Enumerable.Empty<Pin>();
        }
    }
} 