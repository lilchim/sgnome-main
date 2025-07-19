using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using LibrariesService.Providers;
using Microsoft.Extensions.Logging;

namespace LibrariesService;

public class LibrariesAggregator
{
    private readonly ISteamLibrariesProvider _steamProvider;
    private readonly ILogger<LibrariesAggregator> _logger;

    public LibrariesAggregator(
        ISteamLibrariesProvider steamProvider,
        ILogger<LibrariesAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetLibrariesPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();
        var totalGameCount = 0;
        var platformCounts = new Dictionary<string, int>();

        // Ensure we have a valid player ID
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName;
        if (string.IsNullOrEmpty(playerId))
        {
            _logger.LogWarning("Player has no valid identifier for library resolution");
            return pins;
        }

        // Get Steam library pins if available
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var context = new PinContext
            {
                InputNodeId = $"player-{playerId}",
                InputNodeType = "player",
                TargetNodeType = "libraries",
                ApiEndpoint = "/api/libraries/select",
                ApiParameters = new Dictionary<string, object>
                {
                    ["playerId"] = playerId,
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

        // Create main Libraries pin that aggregates all platforms
        if (totalGameCount > 0)
        {
            var mainLibrariesPin = new Pin
            {
                Id = $"libraries-{playerId}",
                Label = $"Libraries ({totalGameCount} games)",
                Type = "libraries",
                Behavior = PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = $"{totalGameCount} games across {platformCounts.Count} platform{(platformCounts.Count > 1 ? "s" : "")}",
                    Count = totalGameCount,
                    Icon = "libraries",
                    Preview = new Dictionary<string, object>
                    {
                        ["platforms"] = platformCounts,
                        ["platformCount"] = platformCounts.Count
                    }
                },
                Metadata = new PinMetadata
                {
                    TargetNodeType = "libraries",
                    OriginNodeId = $"player-{playerId}",
                    ApiEndpoint = "/api/libraries/select",
                    Parameters = new Dictionary<string, object>
                    {
                        ["playerId"] = playerId,
                        ["playerType"] = "steam"
                    }
                }
            };
            
            // Add the main libraries pin at the beginning
            pins.Insert(0, mainLibrariesPin);
        }

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetLibrariesPinsAsync(LibrariesNode libraries)
    {
        var pins = new List<Pin>();

        // Ensure we have a valid player ID
        if (string.IsNullOrEmpty(libraries.PlayerId))
        {
            _logger.LogWarning("Libraries node has no valid player ID for pin resolution");
            return pins;
        }

        // Get informational pins about the libraries itself
        // For now, we'll get Steam library info if the player has a Steam ID
        var context = new PinContext
        {
            InputNodeId = $"libraries-{libraries.PlayerId}",
            InputNodeType = "libraries",
            TargetNodeType = "libraries",
            ApiEndpoint = null, // Informational pin about self
            ApiParameters = new Dictionary<string, object>()
        };

        var steamPins = await _steamProvider.GetLibraryPinsAsync(libraries.PlayerId, context);
        pins.AddRange(steamPins);

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} informational pins for libraries {PlayerId}", 
            pins.Count, libraries.PlayerId);

        return pins;
    }
} 