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
    
    public async Task<PlayerNode> ResolvePlayerAsync(Dictionary<string, string> identifiers)
    {
        _logger.LogDebug("Attempting to resolve player with {IdentifierCount} identifiers", identifiers.Count);
        
        PlayerNode? existingPlayer = null;
        string? foundInternalId = null;
        
        // Try each identifier to find existing player
        foreach (var (identifierType, identifierValue) in identifiers)
        {
            var lookupKey = $"{PlayerKeyPrefix}:{identifierType}:{identifierValue}";
            var internalId = await _redis.StringGetAsync(lookupKey);
            
            if (!internalId.IsNull)
            {
                _logger.LogDebug("Found player via {IdentifierType}: {IdentifierValue} -> {InternalId}", 
                    identifierType, identifierValue, internalId);
                
                foundInternalId = internalId;
                existingPlayer = await GetPlayerByInternalIdAsync(internalId!);
                break; // Found existing player, stop searching
            }
        }
        
        if (existingPlayer != null)
        {
            // Update existing player with any new identifiers provided in this request
            var updated = await UpdatePlayerWithNewIdentifiersAsync(existingPlayer, identifiers);
            if (updated)
            {
                _logger.LogDebug("Updated existing player {InternalId} with new identifiers", foundInternalId);
            }
            return existingPlayer;
        }
        
        // Create new player if not found
        _logger.LogDebug("No existing player found, creating new player");
        return await CreatePlayerAsync(new PlayerNode
        {
            DisplayName = "Unknown Player", // Will be updated by external API calls
            Identifiers = new Dictionary<string, string>()
        }, identifiers);
    }
    
    private async Task<PlayerNode> CreatePlayerAsync(PlayerNode player, Dictionary<string, string> identifiers)
    {
        // Generate internal ID
        var internalId = Guid.NewGuid().ToString();
        
        // Use input identifiers and add internal ID
        player.Identifiers = new Dictionary<string, string>(identifiers);
        player.Identifiers[PlayerIdentifiers.Internal] = internalId;
        player.InternalId = internalId;

        var playerKey = $"{PlayerKeyPrefix}:{InternalKeyPrefix}:{internalId}";
        var playerJson = JsonSerializer.Serialize(player);
        
        // Store player data
        await _redis.StringSetAsync(playerKey, playerJson);
        
        // Create identifier mappings
        await AddIdentifiersAsync(internalId, identifiers);
        
        _logger.LogInformation("Created new player with internal ID {InternalId} and {IdentifierCount} identifiers", 
            internalId, player.Identifiers.Count);
        return player;
    }
    
    private async Task<PlayerNode> UpdatePlayerAsync(PlayerNode player)
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
    
    private async Task<bool> UpdatePlayerWithNewIdentifiersAsync(PlayerNode player, Dictionary<string, string> newIdentifiers)
    {
        var updated = false;
        
        foreach (var (identifierType, identifierValue) in newIdentifiers)
        {
            // Skip internal identifier as it's managed by us
            if (identifierType == PlayerIdentifiers.Internal)
                continue;
                
            // Add new identifier if it doesn't exist
            if (!player.Identifiers.ContainsKey(identifierType))
            {
                player.Identifiers[identifierType] = identifierValue;
                updated = true;
                _logger.LogDebug("Added new identifier {Type}: {Value} to player {InternalId}", 
                    identifierType, identifierValue, player.InternalId);
            }
        }
        
        if (updated)
        {
            // Update the player in database
            await UpdatePlayerAsync(player);
            
            // Add new identifier mappings to Redis
            var internalId = player.Identifiers[PlayerIdentifiers.Internal];
            await AddIdentifiersAsync(internalId, newIdentifiers.Where(kvp => kvp.Key != PlayerIdentifiers.Internal).ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }
        
        return updated;
    }
    
    public async Task AddIdentifiersAsync(string internalId, Dictionary<string, string> identifiers)
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