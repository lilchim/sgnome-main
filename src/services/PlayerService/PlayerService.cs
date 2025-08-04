using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Database;
using PlayerService.PinGenerators;
using Microsoft.Extensions.Logging;

namespace PlayerService;

public class PlayerService : IPlayerService
{
    private readonly IPlayerDatabase _database;
    private readonly PlayerInfoPinGenerator _infoPinGenerator;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerDatabase database,
        PlayerInfoPinGenerator infoPinGenerator,
        ILogger<PlayerService> logger)
    {
        _database = database;
        _infoPinGenerator = infoPinGenerator;
        _logger = logger;
    }

    public async Task<(IEnumerable<Pin> Pins, PlayerNode ResolvedNode)> Consume(PlayerNode partial)
    {
        _logger.LogInformation("Consuming PlayerNode");
        
        try
        {
            // Resolve player from database (creates or updates as needed)
            var resolvedPlayer = await _database.ResolvePlayerAsync(partial.Identifiers);
            _logger.LogInformation("Resolved player with internal ID {InternalId}", resolvedPlayer.InternalId);
            
            // Generate enrichment pins for the resolved player
            var pins = await CreatePlayerPinsAsync(resolvedPlayer, resolvedPlayer.InternalId!, "player");
            
            return (pins, resolvedPlayer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PlayerNode");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> Consume(LibraryNode library)
    {
        _logger.LogInformation("Consuming LibraryNode for player data");
        
        try
        {
            var pins = new List<Pin>();
            
            // For now, we don't have specific player-related pins for libraries
            // This could be expanded to show library owners, recent players, etc.
            
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming LibraryNode for player data");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> Consume(LibraryListNode libraryList)
    {
        _logger.LogInformation("Consuming LibraryListNode for player data");
        
        try
        {
            var pins = new List<Pin>();
            
            // For now, we don't have specific player-related pins for library lists
            // This could be expanded to show collection owners, shared players, etc.
            
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming LibraryListNode for player data");
            throw;
        }
    }

    private async Task<IEnumerable<Pin>> CreatePlayerPinsAsync(PlayerNode player, string originNodeId, string originNodeType)
    {
        var context = new PinContext
        {
            InputNodeId = originNodeId,
            InputNodeType = originNodeType,
            TargetNodeType = "player",
            ApiEndpoint = "/api/player/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["internalId"] = player.InternalId!
            }
        };

        var allPins = new List<Pin>();
        
        // Generate info pins
        var infoPins = await _infoPinGenerator.GeneratePinsAsync(player, context);
        allPins.AddRange(infoPins);
        
        // Generate friends pins

        
        // Generate activity pins

        
        return allPins;
    }
} 