using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using LibraryService.Providers;
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
        _logger.LogInformation("Resolving LibraryNode for {LibrarySource} player {PlayerId}", 
            partialLibrary.LibrarySource, partialLibrary.PlayerId);
        
        try
        {
            // For now: create on the spot (baby steps)
            // Later: check cache, database, fetch from providers, etc.
            var resolvedLibrary = new LibraryNode
            {
                LibrarySource = partialLibrary.LibrarySource,
                PlayerId = partialLibrary.PlayerId,
                DisplayName = partialLibrary.DisplayName ?? $"{partialLibrary.LibrarySource} Library",
                TotalGameCount = partialLibrary.TotalGameCount,
                AvailableCategories = partialLibrary.AvailableCategories ?? new List<string> { "recently-played" },
                LastUpdated = DateTime.UtcNow
            };

            // TODO: Fetch actual data from providers to populate TotalGameCount and AvailableCategories
            // For now, just use what we have
            _logger.LogInformation("LibraryNode resolved for {LibrarySource} player {PlayerId}", 
                partialLibrary.LibrarySource, partialLibrary.PlayerId);

            return resolvedLibrary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving LibraryNode for {LibrarySource} player {PlayerId}", 
                partialLibrary.LibrarySource, partialLibrary.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibraryNode library)
    {
        _logger.LogInformation("Getting library pins for {LibrarySource} player {PlayerId}", 
            library.LibrarySource, library.PlayerId);

        try
        {
            var pins = await _aggregator.GetLibraryPinsAsync(library);
            _logger.LogInformation("Successfully retrieved {PinCount} library pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library pins for {LibrarySource} player {PlayerId}", 
                library.LibrarySource, library.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player)
    {
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
        _logger.LogInformation("Getting library pins for player {PlayerId}", playerId);

        try
        {
            var pins = await _aggregator.GetLibraryPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} library pins for player", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library pins for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibrariesNode libraries)
    {
        _logger.LogInformation("Getting library pins for libraries {PlayerId}", libraries.PlayerId);

        try
        {
            var pins = await _aggregator.GetLibraryPinsAsync(libraries);
            _logger.LogInformation("Successfully retrieved {PinCount} library pins for libraries", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting library pins for libraries {PlayerId}", libraries.PlayerId);
            throw;
        }
    }
} 