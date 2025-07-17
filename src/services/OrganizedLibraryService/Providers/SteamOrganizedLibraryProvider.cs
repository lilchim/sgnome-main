using SteamApi.Models.Steam.Responses;
using SteamApi.Client;
using Microsoft.Extensions.Logging;
using Sgnome.Models.Graph;

namespace OrganizedLibraryService.Providers;

public class SteamOrganizedLibraryProvider : ISteamOrganizedLibraryProvider
{
    private readonly ISteamApiClient _steamApiClient;
    private readonly ILogger<SteamOrganizedLibraryProvider> _logger;

    public SteamOrganizedLibraryProvider(
        ISteamApiClient steamApiClient,
        ILogger<SteamOrganizedLibraryProvider> logger)
    {
        _steamApiClient = steamApiClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(string steamId, PinContext context)
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
            var pins = new List<Pin>();

            // Steam organized library pin
            var steamOrganizedLibraryPin = new Pin
            {
                Id = $"steam-organized-library-{steamId}",
                Label = $"Steam Organized Library ({ownedGames.GameCount} games)",
                Type = "steam-organized-library",
                Behavior = context.InputNodeType == "organized-library" ? PinBehavior.Informational : PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = $"{ownedGames.GameCount} games available for organization",
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
            pins.Add(steamOrganizedLibraryPin);

            // TODO: When GamesListService is created, it will be responsible for creating
            // pins that point to games-list nodes. This service only provides information
            // about the organized library itself.

            _logger.LogInformation("Retrieved {PinCount} organized library pins for Steam {SteamId}", 
                pins.Count, steamId);

            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organized library pins for Steam {SteamId}", steamId);
            return Enumerable.Empty<Pin>();
        }
    }
}  