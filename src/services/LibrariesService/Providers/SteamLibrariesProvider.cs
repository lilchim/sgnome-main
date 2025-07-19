using SteamApi.Models.Steam.Responses;
using SteamApi.Client;
using Microsoft.Extensions.Logging;
using Sgnome.Models.Graph;

namespace LibrariesService.Providers;

public class SteamLibrariesProvider : ISteamLibrariesProvider
{
    private readonly ISteamApiClient _steamApiClient;
    private readonly ILogger<SteamLibrariesProvider> _logger;

    public SteamLibrariesProvider(
        ISteamApiClient steamApiClient,
        ILogger<SteamLibrariesProvider> logger)
    {
        _steamApiClient = steamApiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(string steamId, PinContext context)
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
                Behavior = context.InputNodeType == "library" ? PinBehavior.Informational : PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = $"{ownedGames.GameCount} games on Steam",
                    Count = ownedGames.GameCount,
                    Icon = "steam"
                },
                Metadata = new PinMetadata
                {
                    TargetNodeType = context.TargetNodeType,
                    OriginNodeId = context.InputNodeId,
                    ApiEndpoint = context.ApiEndpoint,
                    Parameters = context.ApiParameters
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