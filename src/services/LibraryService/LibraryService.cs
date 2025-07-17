using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Microsoft.Extensions.Logging;

namespace LibraryService;

public class LibraryService : ILibraryService
{
    private readonly LibraryAggregator _aggregator;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        LibraryAggregator aggregator,
        ILogger<LibraryService> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<LibraryNode> ResolveNodeAsync(LibraryNode partialLibrary)
    {
        _logger.LogInformation("Resolving LibraryNode for player {PlayerId}", partialLibrary.PlayerId);
        
        try
        {
            // For now: create on the spot (baby steps)
            // Later: check cache, database, aggregate from providers, etc.
            var resolvedLibrary = new LibraryNode
            {
                PlayerId = partialLibrary.PlayerId,
                DisplayName = partialLibrary.DisplayName ?? "Game Libraries",         AvailableSources = partialLibrary.AvailableSources ?? new List<string> { "steam" },
                TotalGameCount = partialLibrary.TotalGameCount,
                LastUpdated = DateTime.UtcNow
            };

            // TODO: Aggregate game counts from all available sources
            // For now, just use what we have
            _logger.LogInformation("LibraryNode resolved for player {PlayerId}", partialLibrary.PlayerId);

            return resolvedLibrary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving LibraryNode for player {PlayerId}", partialLibrary.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting library pins for player {PlayerId}", player.SteamId);
        
        try
        {
            var pins = await _aggregator.GetLibraryPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} library pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }
} 