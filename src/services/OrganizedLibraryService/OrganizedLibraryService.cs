using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using OrganizedLibraryService.Providers;
using Microsoft.Extensions.Logging;

namespace OrganizedLibraryService;

public class OrganizedLibraryService : IOrganizedLibraryService
{
    private readonly OrganizedLibraryAggregator _aggregator;
    private readonly ILogger<OrganizedLibraryService> _logger;

    public OrganizedLibraryService(
        OrganizedLibraryAggregator aggregator,
        ILogger<OrganizedLibraryService> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<OrganizedLibraryNode> ResolveNodeAsync(OrganizedLibraryNode partialOrganizedLibrary)
    {
        _logger.LogInformation("Resolving OrganizedLibraryNode for {LibrarySource} player {PlayerId}", 
            partialOrganizedLibrary.LibrarySource, partialOrganizedLibrary.PlayerId);
        
        try
        {
            // For now: create on the spot (baby steps)
            // Later: check cache, database, fetch from providers, etc.
            var resolvedOrganizedLibrary = new OrganizedLibraryNode
            {
                LibrarySource = partialOrganizedLibrary.LibrarySource,
                PlayerId = partialOrganizedLibrary.PlayerId,
                DisplayName = partialOrganizedLibrary.DisplayName ?? $"{partialOrganizedLibrary.LibrarySource} Library",
                TotalGameCount = partialOrganizedLibrary.TotalGameCount,
                AvailableCategories = partialOrganizedLibrary.AvailableCategories ?? new List<string> { "recently-played" },
                LastUpdated = DateTime.UtcNow
            };

            // TODO: Fetch actual data from providers to populate TotalGameCount and AvailableCategories
            // For now, just use what we have
            _logger.LogInformation("OrganizedLibraryNode resolved for {LibrarySource} player {PlayerId}", 
                partialOrganizedLibrary.LibrarySource, partialOrganizedLibrary.PlayerId);

            return resolvedOrganizedLibrary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving OrganizedLibraryNode for {LibrarySource} player {PlayerId}", 
                partialOrganizedLibrary.LibrarySource, partialOrganizedLibrary.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(OrganizedLibraryNode organizedLibrary)
    {
        _logger.LogInformation("Getting organized library pins for {LibrarySource} player {PlayerId}", 
            organizedLibrary.LibrarySource, organizedLibrary.PlayerId);

        try
        {
            var pins = await _aggregator.GetOrganizedLibraryPinsAsync(organizedLibrary);
            _logger.LogInformation("Successfully retrieved {PinCount} organized library pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organized library pins for {LibrarySource} player {PlayerId}", 
                organizedLibrary.LibrarySource, organizedLibrary.PlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting organized library pins for player {PlayerId}", player.SteamId);

        try
        {
            var pins = await _aggregator.GetOrganizedLibraryPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} organized library pins for player", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organized library pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(LibraryNode library)
    {
        _logger.LogInformation("Getting organized library pins for library {PlayerId}", library.PlayerId);

        try
        {
            var pins = await _aggregator.GetOrganizedLibraryPinsAsync(library);
            _logger.LogInformation("Successfully retrieved {PinCount} organized library pins for library", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting organized library pins for library {PlayerId}", library.PlayerId);
            throw;
        }
    }
} 