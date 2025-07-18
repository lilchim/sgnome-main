using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;
using Microsoft.Extensions.Logging;

namespace PlayerService;

public class PlayerService : IPlayerService
{
    private readonly PlayerAggregator _aggregator;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        PlayerAggregator aggregator,
        ILogger<PlayerService> logger)
    {
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<PlayerNode> ResolveNodeAsync(PlayerNode partialPlayer)
    {
        _logger.LogInformation("Resolving PlayerNode for SteamId {SteamId}", partialPlayer.SteamId);
        
        try
        {
            // For now: create on the spot (baby steps)
            // Later: check cache, database, external APIs, etc.
            var resolvedPlayer = new PlayerNode
            {
                SteamId = partialPlayer.SteamId,
                EpicId = partialPlayer.EpicId,
                DisplayName = partialPlayer.DisplayName,
                AvatarUrl = partialPlayer.AvatarUrl,
                Identifiers = new Dictionary<string, object>()
            };

            // If we have a SteamId, try to get additional data from providers
            if (!string.IsNullOrEmpty(partialPlayer.SteamId))
            {
                // TODO: Call providers to enrich the player data
                // For now, just use what we have
                _logger.LogInformation("PlayerNode resolved for SteamId {SteamId}", partialPlayer.SteamId);
            }

            return resolvedPlayer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving PlayerNode for SteamId {SteamId}", partialPlayer.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetPlayerInfoPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting player info pins for player {PlayerId}", player.SteamId);
        
        try
        {
            var pins = await _aggregator.GetPlayerInfoPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} player info pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player info pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetFriendsPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting friends pins for player {PlayerId}", player.SteamId);
        
        try
        {
            var pins = await _aggregator.GetFriendsPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} friends pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting friends pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> GetActivityPinsAsync(PlayerNode player)
    {
        _logger.LogInformation("Getting activity pins for player {PlayerId}", player.SteamId);
        
        try
        {
            var pins = await _aggregator.GetActivityPinsAsync(player);
            _logger.LogInformation("Successfully retrieved {PinCount} activity pins", pins.Count());
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity pins for player {PlayerId}", player.SteamId);
            throw;
        }
    }
} 