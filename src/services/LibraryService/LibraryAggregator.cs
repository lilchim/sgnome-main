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

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibraryNode library)
    {
        var pins = new List<Pin>();

        // Create context for self-reference (informational pins)
        var context = new PinContext
        {
            InputNodeId = $"library-{library.LibrarySource}-{library.PlayerId}",
            InputNodeType = "library",
            TargetNodeType = "library",
            ApiEndpoint = null, // Informational pins don't have API endpoints
            ApiParameters = new Dictionary<string, object>()
        };

        // Get library pins based on the library source
        if (library.LibrarySource.Equals("steam", StringComparison.OrdinalIgnoreCase))
        {
            var steamPins = await _steamProvider.GetLibraryPinsAsync(library.PlayerId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Aggregated {PinCount} library pins for {LibrarySource} player {PlayerId}", 
            pins.Count, library.LibrarySource, library.PlayerId);

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";

        // Create context for cross-domain (relational pins from player)
        var context = new PinContext
        {
            InputNodeId = $"player-{playerId}",
            InputNodeType = "player",
            TargetNodeType = "library",
            ApiEndpoint = "/api/library/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = playerId,
                ["librarySource"] = "steam"
            }
        };

        // Get library pins for a player
        // For now, we'll create Steam library pins if the player has a Steam ID
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var steamPins = await _steamProvider.GetLibraryPinsAsync(player.SteamId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} library pins for player {PlayerId}", 
            pins.Count, playerId);

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibrariesNode libraries)
    {
        var pins = new List<Pin>();

        // Create context for cross-domain (relational pins from libraries collection)
        var context = new PinContext
        {
            InputNodeId = $"libraries-{libraries.PlayerId}",
            InputNodeType = "libraries",
            TargetNodeType = "library",
            ApiEndpoint = "/api/library/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = libraries.PlayerId,
                ["librarySource"] = "steam"
            }
        };

        // Get library pins for a libraries collection
        // For now, we'll create Steam library pins if the libraries has a player ID
        if (!string.IsNullOrEmpty(libraries.PlayerId))
        {
            var steamPins = await _steamProvider.GetLibraryPinsAsync(libraries.PlayerId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} library pins for libraries {PlayerId}", 
            pins.Count, libraries.PlayerId);

        return pins;
    }
} 