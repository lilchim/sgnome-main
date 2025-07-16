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

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(string librarySource, string playerId)
    {
        _logger.LogInformation("Getting organized library pins for {LibrarySource} player {PlayerId}", librarySource, playerId);
        
        try
        {
            var pins = await _aggregator.GetOrganizedLibraryPinsAsync(librarySource, playerId);
            _logger.LogInformation("Successfully retrieved {PinCount} organized library pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organized library pins for {LibrarySource} player {PlayerId}", librarySource, playerId);
            throw;
        }
    }
} 