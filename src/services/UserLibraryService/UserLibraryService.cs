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

    public async Task<GraphResponse> GetUserLibraryAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting user library for player {PlayerId}", player.SteamId);
        
        try
        {
            var response = await _aggregator.GetUserLibraryAsync(player);
            _logger.LogInformation("Successfully retrieved library with {NodeCount} nodes and {EdgeCount} edges", 
                response.Nodes.Count, response.Edges.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user library for player {PlayerId}", player.SteamId);
            throw;
        }
    }

    public async Task<GraphResponse> GetRecentlyPlayedGamesAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting recently played games for player {PlayerId}", player.SteamId);
        
        try
        {
            var response = await _aggregator.GetRecentlyPlayedGamesAsync(player);
            _logger.LogInformation("Successfully retrieved {NodeCount} recently played games", response.Nodes.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recently played games for player {PlayerId}", player.SteamId);
            throw;
        }
    }
} 