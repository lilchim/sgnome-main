using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Microsoft.Extensions.Logging;

namespace UserLibraryService;

public class UserLibraryService : IUserLibraryService
{
    private readonly UserLibraryAggregator _aggregator;
    private readonly ILogger<UserLibraryService> _logger;

    public UserLibraryService(
        UserLibraryAggregator aggregator,
        ILogger<UserLibraryService> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetUserLibraryPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting user library pins for player {PlayerId}", player.SteamId);
        
        try
        {
            var pins = await _aggregator.GetUserLibraryPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} library pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user library pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }
} 