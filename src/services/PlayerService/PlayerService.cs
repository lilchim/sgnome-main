using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;
using PlayerService.Database;
using Microsoft.Extensions.Logging;

namespace PlayerService;

public class PlayerService : IPlayerService
{
    private readonly PlayerAggregator _aggregator;
    private readonly IPlayerDatabase _database;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        PlayerAggregator aggregator,
        IPlayerDatabase database,
        ILogger<PlayerService> logger)
    {
        _aggregator = aggregator;
        _database = database;
        _logger = logger;
    }

    public async Task<PlayerNode> ResolveNodeAsync(PlayerNode partialPlayer)
    {
        // Extract Steam ID from identifiers
        // var steamId = partialPlayer.Identifiers.GetValueOrDefault(PlayerIdentifiers.Steam);
        
        _logger.LogInformation("Resolving PlayerNode");
        
        try
        {
            // Resolve player from database (creates or updates as needed)
            var resolvedPlayer = await _database.ResolvePlayerAsync(partialPlayer.Identifiers);
            _logger.LogInformation("Resolved player with internal ID {InternalId}", resolvedPlayer.InternalId);
            
            return resolvedPlayer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving PlayerNode");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetPlayerInfoPinsAsync(PlayerNode player)
    {
        if (string.IsNullOrEmpty(player.InternalId))
        {
            _logger.LogWarning("Player missing InternalId, cannot process player info pins");
            return Enumerable.Empty<Pin>();
        }

        var playerId = player.InternalId;
        _logger.LogInformation("Getting player info pins for player {PlayerId}", playerId);
        
        try
        {
            var pins = await _aggregator.GetPlayerInfoPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} player info pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player info pins for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetFriendsPinsAsync(PlayerNode player)
    {
        if (string.IsNullOrEmpty(player.InternalId))
        {
            _logger.LogWarning("Player missing InternalId, cannot process friends pins");
            return Enumerable.Empty<Pin>();
        }

        var playerId = player.InternalId;
        _logger.LogInformation("Getting friends pins for player {PlayerId}", playerId);
        
        try
        {
            var pins = await _aggregator.GetFriendsPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} friends pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends pins for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetActivityPinsAsync(PlayerNode player)
    {
        if (string.IsNullOrEmpty(player.InternalId))
        {
            _logger.LogWarning("Player missing InternalId, cannot process activity pins");
            return Enumerable.Empty<Pin>();
        }

        var playerId = player.InternalId;
        _logger.LogInformation("Getting activity pins for player {PlayerId}", playerId);
        
        try
        {
            var pins = await _aggregator.GetActivityPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} activity pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity pins for player {PlayerId}", playerId);
            throw;
        }
    }
} 