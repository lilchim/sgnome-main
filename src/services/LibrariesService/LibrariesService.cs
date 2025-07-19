using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Microsoft.Extensions.Logging;

namespace LibrariesService;

public class LibrariesService : ILibrariesService
{
    private readonly LibrariesAggregator _aggregator;
    private readonly ILogger<LibrariesService> _logger;

    public LibrariesService(
        LibrariesAggregator aggregator,
        ILogger<LibrariesService> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<LibrariesNode> ResolveNodeAsync(LibrariesNode partialLibraries)
    {
        _logger.LogInformation("Resolving LibrariesNode for player {PlayerId}", partialLibraries.PlayerId);

        try
        {
            // For now: create on the spot (baby steps)
            // Later: check cache, database, aggregate from providers, etc.
            var resolvedLibraries = new LibrariesNode
            {
                PlayerId = partialLibraries.PlayerId,
                DisplayName = partialLibraries.DisplayName ?? "Game Libraries",
                AvailableSources = partialLibraries.AvailableSources ?? new List<string> { "steam" },
                TotalGameCount = partialLibraries.TotalGameCount,
                LastUpdated = DateTime.UtcNow
            };

            // TODO: Aggregate game counts from all available sources
            // For now, just use what we have
            _logger.LogInformation("LibrariesNode resolved for player {PlayerId}", partialLibraries.PlayerId);

            return resolvedLibraries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving LibrariesNode for player {PlayerId}", partialLibraries.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibrariesPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting libraries pins for player {PlayerId}", player.SteamId);

        try
        {
            var pins = await _aggregator.GetLibrariesPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} libraries pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting libraries pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibrariesPinsAsync(LibrariesNode libraries)
    {
        _logger.LogInformation("Getting libraries pins for libraries {PlayerId}", libraries.PlayerId);

        try
        {
            var pins = await _aggregator.GetLibrariesPinsAsync(libraries);
            _logger.LogInformation("Successfully retrieved {PinCount} libraries informational pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting libraries pins for libraries {PlayerId}", libraries.PlayerId);
            throw;
        }
    }
}