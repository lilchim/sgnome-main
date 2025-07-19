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
        _logger.LogInformation("Resolving PlayerNode for SteamId {SteamId}", partialPlayer.SteamId);
        
        try
        {
            // Build identifiers dictionary for lookup
            var identifiers = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(partialPlayer.SteamId))
                identifiers[PlayerIdentifiers.Steam] = partialPlayer.SteamId;
            if (!string.IsNullOrEmpty(partialPlayer.EpicId))
                identifiers[PlayerIdentifiers.Epic] = partialPlayer.EpicId;
            
            // Try to resolve existing player from database
            var existingPlayer = await _database.ResolvePlayerAsync(identifiers);
            if (existingPlayer != null)
            {
                // Set InternalId for easier access
                existingPlayer.InternalId = existingPlayer.Identifiers.GetValueOrDefault(PlayerIdentifiers.Internal)?.ToString();
                _logger.LogInformation("Found existing player with internal ID {InternalId}", existingPlayer.InternalId);
                return existingPlayer;
            }
            
            // Create new player if not found
            var newPlayer = new PlayerNode
            {
                SteamId = partialPlayer.SteamId,
                EpicId = partialPlayer.EpicId,
                DisplayName = partialPlayer.DisplayName,
                AvatarUrl = partialPlayer.AvatarUrl,
                Identifiers = new Dictionary<string, object>()
            };
            
            var createdPlayer = await _database.CreatePlayerAsync(newPlayer, identifiers);
            // Set InternalId for easier access
            createdPlayer.InternalId = createdPlayer.Identifiers.GetValueOrDefault(PlayerIdentifiers.Internal)?.ToString();
            _logger.LogInformation("Created new player with internal ID {InternalId}", createdPlayer.InternalId);
            
            return createdPlayer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving PlayerNode for SteamId {SteamId}", partialPlayer.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetPlayerInfoPinsAsync(PlayerNode player)
    {
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
        _logger.LogInformation("Getting player info pins for player {PlayerId}", playerId);
        
        try
        {
            var pins = await _aggregator.GetPlayerInfoPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} player info pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            var errorPlayerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
            _logger.LogError(ex, "Error getting player info pins for player {PlayerId}", errorPlayerId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetFriendsPinsAsync(PlayerNode player)
    {
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
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
        var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
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