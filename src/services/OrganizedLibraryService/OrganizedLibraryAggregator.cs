using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using OrganizedLibraryService.Providers;
using Microsoft.Extensions.Logging;

namespace OrganizedLibraryService;

public class OrganizedLibraryAggregator
{
    private readonly ISteamOrganizedLibraryProvider _steamProvider;
    private readonly ILogger<OrganizedLibraryAggregator> _logger;

    public OrganizedLibraryAggregator(
        ISteamOrganizedLibraryProvider steamProvider,
        ILogger<OrganizedLibraryAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(OrganizedLibraryNode organizedLibrary)
    {
        var pins = new List<Pin>();

        // Create context for self-reference (informational pins)
        var context = new PinContext
        {
            InputNodeId = $"organized-library-{organizedLibrary.LibrarySource}-{organizedLibrary.PlayerId}",
            InputNodeType = "organized-library",
            TargetNodeType = "organized-library",
            ApiEndpoint = null, // Informational pins don't have API endpoints
            ApiParameters = new Dictionary<string, object>()
        };

        // Get organized library pins based on the library source
        if (organizedLibrary.LibrarySource.Equals("steam", StringComparison.OrdinalIgnoreCase))
        {
            var steamPins = await _steamProvider.GetOrganizedLibraryPinsAsync(organizedLibrary.PlayerId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Aggregated {PinCount} organized library pins for {LibrarySource} player {PlayerId}", 
            pins.Count, organizedLibrary.LibrarySource, organizedLibrary.PlayerId);

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Create context for cross-domain (relational pins from player)
        var context = new PinContext
        {
            InputNodeId = $"player-{player.SteamId}",
            InputNodeType = "player",
            TargetNodeType = "organized-library",
            ApiEndpoint = "/api/organized-library/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = player.SteamId ?? string.Empty,
                ["librarySource"] = "steam"
            }
        };

        // Get organized library pins for a player
        // For now, we'll create Steam organized library pins if the player has a Steam ID
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var steamPins = await _steamProvider.GetOrganizedLibraryPinsAsync(player.SteamId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} organized library pins for player {PlayerId}", 
            pins.Count, player.SteamId);

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(LibraryNode library)
    {
        var pins = new List<Pin>();

        // Create context for cross-domain (relational pins from library)
        var context = new PinContext
        {
            InputNodeId = $"library-{library.PlayerId}",
            InputNodeType = "library",
            TargetNodeType = "organized-library",
            ApiEndpoint = "/api/organized-library/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = library.PlayerId,
                ["librarySource"] = "steam"
            }
        };

        // Get organized library pins for a library
        // For now, we'll create Steam organized library pins if the library has a player ID
        if (!string.IsNullOrEmpty(library.PlayerId))
        {
            var steamPins = await _steamProvider.GetOrganizedLibraryPinsAsync(library.PlayerId, context);
            pins.AddRange(steamPins);
        }

        // TODO: Add Epic, GOG, etc. providers as they become available

        _logger.LogInformation("Retrieved {PinCount} organized library pins for library {PlayerId}", 
            pins.Count, library.PlayerId);

        return pins;
    }
} 