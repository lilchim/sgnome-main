using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using LibraryService.Providers;
using Microsoft.Extensions.Logging;

namespace LibraryService;

public class LibraryAggregator
{
    private readonly ISteamLibraryProvider _steamProvider;
    private readonly ILogger<LibraryAggregator> _logger;

    public LibraryAggregator(
        ISteamLibraryProvider steamProvider,
        ILogger<LibraryAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Get Steam library pins if available
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var steamPins = await _steamProvider.GetLibraryPinsAsync(player.SteamId);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available
        // TODO: Create top level library pin (e.g. aggregated game count)

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(string librarySource, string playerId)
    {
        var pins = new List<Pin>();

        if (librarySource.Equals("steam", StringComparison.OrdinalIgnoreCase))
        {
            // For organized library, we need to get the raw Steam data to create specific pins
            // This is a temporary approach - in the future, we might want a separate method
            // that returns raw data for organized library creation
            var steamPins = await _steamProvider.GetLibraryPinsAsync(playerId);
            
            // For now, we'll create organized library pins based on the steam library pin
            // In a more sophisticated implementation, we might want the provider to return
            // both the library pin and organized library pins, or have a separate method
            foreach (var steamPin in steamPins)
            {
                if (steamPin.Type == "library")
                {
                    pins.AddRange(CreateOrganizedLibraryPins(playerId, steamPin));
                }
            }
        }

        // TODO: Add other library sources

        return pins;
    }

    private IEnumerable<Pin> CreateOrganizedLibraryPins(string playerId, Pin steamLibraryPin)
    {
        var pins = new List<Pin>();

        // Recently played games pin
        var recentlyPlayedPin = new Pin
        {
            Id = "recently-played",
            Label = "Recently Played",
            Type = "games-list",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = "View recently played games",
                Icon = "clock"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = "games-list",
                OriginNodeId = $"organized-library-steam-{playerId}",
                ApiEndpoint = "/api/games-list/recently-played",
                Parameters = new Dictionary<string, object>
                {
                    ["steamId"] = playerId,
                    ["source"] = "steam",
                    ["listType"] = "recently-played"
                }
            }
        };
        pins.Add(recentlyPlayedPin);

        // All games pin
        var allGamesPin = new Pin
        {
            Id = "all-games",
            Label = $"All Games ({steamLibraryPin.Summary?.Count ?? 0})",
            Type = "games-list",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = $"All {steamLibraryPin.Summary?.Count ?? 0} games in library",
                Count = steamLibraryPin.Summary?.Count ?? 0,
                Icon = "library"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = "games-list",
                OriginNodeId = $"organized-library-steam-{playerId}",
                ApiEndpoint = "/api/games-list/all",
                Parameters = new Dictionary<string, object>
                {
                    ["steamId"] = playerId,
                    ["source"] = "steam",
                    ["listType"] = "all-games"
                }
            }
        };
        pins.Add(allGamesPin);

        return pins;
    }
} 