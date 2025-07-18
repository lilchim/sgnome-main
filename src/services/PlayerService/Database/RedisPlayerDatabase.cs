using StackExchange.Redis;
using System.Text.Json;
using Sgnome.Models.Nodes;
using Microsoft.Extensions.Logging;

namespace PlayerService.Database;

/// <summary>
/// Redis implementation of player node persistence
/// </summary>
public class RedisPlayerDatabase : IPlayerDatabase
{
    private readonly IDatabase _redis;
    private readonly ILogger<RedisPlayerDatabase> _logger;
    private const string PlayerKeyPrefix = "player";
    private const string InternalKeyPrefix = "internal";
    
    public RedisPlayerDatabase(IConnectionMultiplexer redis, ILogger<RedisPlayerDatabase> logger)
    {
        _redis = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<PlayerNode?> ResolvePlayerAsync(Dictionary<string, object> identifiers)
    {
        _logger.LogDebug("Attempting to resolve player with {IdentifierCount} identifiers", identifiers.Count);
        
        // Try each identifier to find existing player
        foreach (var (identifierType, identifierValue) in identifiers)
        {
            var lookupKey = $"{PlayerKeyPrefix}:{identifierType}:{identifierValue}";
            var internalId = await _redis.StringGetAsync(lookupKey);
            
            if (!internalId.IsNull)
            {
                _logger.LogDebug("Found player via {IdentifierType}: {IdentifierValue} -> {InternalId}", 
                    identifierType, identifierValue, internalId);
                
                return await GetPlayerByInternalIdAsync(internalId!);
            }
        }
        
        _logger.LogDebug("No existing player found for provided identifiers");
        return null;
    }
    
    public async Task<PlayerNode> CreatePlayerAsync(PlayerNode player, Dictionary<string, object> identifiers)
    {
        // Generate internal ID
        var internalId = Guid.NewGuid().ToString();
        player.Identifiers[PlayerIdentifiers.Internal] = internalId;
        
        var playerKey = $"{PlayerKeyPrefix}:{InternalKeyPrefix}:{internalId}";
        var playerJson = JsonSerializer.Serialize(player);
        
        // Store player data
        await _redis.StringSetAsync(playerKey, playerJson);
        
        // Create identifier mappings
        await AddIdentifiersAsync(internalId, identifiers);
        
        _logger.LogInformation("Created new player with internal ID {InternalId}", internalId);
        return player;
    }
    
    public async Task<PlayerNode> UpdatePlayerAsync(PlayerNode player)
    {
        if (!player.Identifiers.TryGetValue(PlayerIdentifiers.Internal, out var internalIdObj) || 
            internalIdObj is not string internalId)
        {
            throw new ArgumentException("Player must have an internal ID to update");
        }
        
        var playerKey = $"{PlayerKeyPrefix}:{InternalKeyPrefix}:{internalId}";
        var playerJson = JsonSerializer.Serialize(player);
        
        await _redis.StringSetAsync(playerKey, playerJson);
        
        _logger.LogDebug("Updated player {InternalId}", internalId);
        return player;
    }
    
    public async Task AddIdentifiersAsync(string internalId, Dictionary<string, object> identifiers)
    {
        var batch = _redis.CreateBatch();
        var tasks = new List<Task>();
        
        foreach (var (identifierType, identifierValue) in identifiers)
        {
            var lookupKey = $"{PlayerKeyPrefix}:{identifierType}:{identifierValue}";
            tasks.Add(batch.StringSetAsync(lookupKey, internalId));
        }
        
        batch.Execute();
        await Task.WhenAll(tasks);
        
        _logger.LogDebug("Added {IdentifierCount} identifier mappings for player {InternalId}", 
            identifiers.Count, internalId);
    }
    
    private async Task<PlayerNode?> GetPlayerByInternalIdAsync(string internalId)
    {
        var playerKey = $"{PlayerKeyPrefix}:{InternalKeyPrefix}:{internalId}";
        var playerJson = await _redis.StringGetAsync(playerKey);
        
        if (playerJson.IsNull)
        {
            _logger.LogWarning("Player data not found for internal ID {InternalId}", internalId);
            return null;
        }
        
        return JsonSerializer.Deserialize<PlayerNode>(playerJson!);
    }
} 