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
        var totalGameCount = 0;
        var platformCounts = new Dictionary<string, int>();

        // Get Steam library pins if available
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var context = new PinContext
            {
                InputNodeId = $"player-{player.SteamId}",
                InputNodeType = "player",
                TargetNodeType = "library",
                ApiEndpoint = "/api/library/select",
                ApiParameters = new Dictionary<string, object>
                {
                    ["playerId"] = player.SteamId,
                    ["playerType"] = "steam"
                }
            };

            var steamPins = await _steamProvider.GetLibraryPinsAsync(player.SteamId, context);
            pins.AddRange(steamPins);
            
            // Extract game count for aggregation
            foreach (var steamPin in steamPins)
            {
                if (steamPin.Type == "steam-library" && steamPin.Summary?.Count.HasValue == true)
                {
                    totalGameCount += steamPin.Summary.Count.Value;
                    platformCounts["Steam"] = steamPin.Summary.Count.Value;
                }
            }
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        // Create main Library pin that aggregates all platforms
        if (totalGameCount > 0)
        {
            var mainLibraryPin = new Pin
            {
                Id = $"library-{player.SteamId ?? player.DisplayName}",
                Label = $"Library ({totalGameCount} games)",
                Type = "library",
                Behavior = PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = $"{totalGameCount} games across {platformCounts.Count} platform{(platformCounts.Count >1 ? "s" : "")}",
                    Count = totalGameCount,
                    Icon = "library",
                    Preview = new Dictionary<string, object>
                    {
                        ["platforms"] = platformCounts,
                        ["platformCount"] = platformCounts.Count
                    }
                },
                Metadata = new PinMetadata
                {
                    TargetNodeType = "library",
                    OriginNodeId = $"player-{player.SteamId ?? player.DisplayName}",
                    ApiEndpoint = "/api/library/select"!,
                    Parameters = new Dictionary<string, object>
                    {
                        ["playerId"] = player.SteamId ?? player.DisplayName,
                        ["playerType"] = "steam"
                    }
                }
            };
            
            // Add the main library pin at the beginning
            pins.Insert(0, mainLibraryPin);
        }

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibraryNode library)
    {
        var pins = new List<Pin>();

        // Get informational pins about the library itself
        // For now, we'll get Steam library info if the player has a Steam ID
        if (!string.IsNullOrEmpty(library.PlayerId))
        {
            var context = new PinContext
            {
                InputNodeId = $"library-{library.PlayerId}",
                InputNodeType = "library",
                TargetNodeType = "library",
                ApiEndpoint = null, // Informational pin about self
                ApiParameters = new Dictionary<string, object>()
            };

            var steamPins = await _steamProvider.GetLibraryPinsAsync(library.PlayerId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} informational pins for library {PlayerId}", 
            pins.Count, library.PlayerId);

        return pins;
    }
} 