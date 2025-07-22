using StackExchange.Redis;
using Sgnome.Models.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LibraryService.Database;

/// <summary>
/// Redis implementation of library list node persistence with Get/Create functionality
/// </summary>
public class RedisLibraryListDatabase : ILibraryListDatabase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisLibraryListDatabase> _logger;

    public RedisLibraryListDatabase(
        IConnectionMultiplexer redis,
        ILogger<RedisLibraryListDatabase> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <summary>
    /// MegaUpsert: Get existing library list or create new one for player
    /// </summary>
    public async Task<LibraryListNode> ResolveLibraryListAsync(string playerId, string? displayName = null)
    {
        _logger.LogDebug("Resolving library list for player {PlayerId}", playerId);

        // GET: Try to find existing library list by player ID
        var existingLibraryList = await FindExistingLibraryListAsync(playerId);
        
        if (existingLibraryList != null)
        {
            // Found existing library list - return as-is (library lists are created with complete mappings)
            _logger.LogDebug("Found existing library list {InternalId} for player {PlayerId}", 
                existingLibraryList.InternalId, playerId);
            return existingLibraryList;
        }

        // CREATE: Generate new library list for player
        _logger.LogDebug("Creating new library list for player {PlayerId}", playerId);
        var newLibraryList = new LibraryListNode
        {
            PlayerId = playerId,
            DisplayName = displayName ?? "Game Libraries",
            LibrarySourceMapping = new Dictionary<string, string>(), // Start empty, will be populated by handlers
            LastUpdated = DateTime.UtcNow
        };

        // Generate internal ID and store
        newLibraryList.InternalId = $"librarylist-{Guid.NewGuid():N}";
        await StoreLibraryListAsync(newLibraryList);

        _logger.LogInformation("Created new library list {InternalId} for player {PlayerId}", 
            newLibraryList.InternalId, playerId);
        
        return newLibraryList;
    }

    /// <summary>
    /// Adds additional library source mappings to an existing library list
    /// </summary>
    public async Task AddLibrarySourceMappingsAsync(string internalId, Dictionary<string, string> librarySourceMappings)
    {
        var db = _redis.GetDatabase();
        var nodeKey = $"librarylist:{internalId}";
        
        var nodeJson = await db.StringGetAsync(nodeKey);
        if (nodeJson.IsNull)
        {
            throw new InvalidOperationException($"LibraryListNode {internalId} not found");
        }

        var libraryList = JsonSerializer.Deserialize<LibraryListNode>(nodeJson!)!;
        
        // Add new mappings
        foreach (var (source, libraryId) in librarySourceMappings)
        {
            if (!libraryList.LibrarySourceMapping.ContainsKey(source))
            {
                libraryList.LibrarySourceMapping[source] = libraryId;
                _logger.LogDebug("Added library mapping {Source} -> {LibraryId} to library list {InternalId}",
                    source, libraryId, internalId);
            }
        }

        libraryList.LastUpdated = DateTime.UtcNow;

        // Store updated node
        var updatedJson = JsonSerializer.Serialize(libraryList);
        await db.StringSetAsync(nodeKey, updatedJson);

        _logger.LogInformation("Added {MappingCount} library source mappings to library list {InternalId}", 
            librarySourceMappings.Count, internalId);
    }

    #region Private Helper Methods

    private async Task<LibraryListNode?> FindExistingLibraryListAsync(string playerId)
    {
        var db = _redis.GetDatabase();
        var playerLookupKey = $"librarylist:player:{playerId}";
        
        var internalId = await db.StringGetAsync(playerLookupKey);
        if (internalId.IsNull)
        {
            return null;
        }

        _logger.LogDebug("Found library list via player {PlayerId} -> {InternalId}", playerId, internalId);
        return await GetLibraryListByInternalIdAsync(internalId!);
    }

    private async Task StoreLibraryListAsync(LibraryListNode libraryList)
    {
        var db = _redis.GetDatabase();
        
        // Store the main node data
        var nodeKey = $"librarylist:{libraryList.InternalId}";
        var nodeJson = JsonSerializer.Serialize(libraryList);
        await db.StringSetAsync(nodeKey, nodeJson);

        // Store player ID lookup
        if (!string.IsNullOrEmpty(libraryList.PlayerId))
        {
            var playerLookupKey = $"librarylist:player:{libraryList.PlayerId}";
            await db.StringSetAsync(playerLookupKey, libraryList.InternalId);
        }
    }

    private async Task<LibraryListNode?> GetLibraryListByInternalIdAsync(string internalId)
    {
        var db = _redis.GetDatabase();
        var nodeKey = $"librarylist:{internalId}";
        
        var nodeJson = await db.StringGetAsync(nodeKey);
        if (nodeJson.IsNull)
        {
            return null;
        }

        return JsonSerializer.Deserialize<LibraryListNode>(nodeJson!);
    }

    #endregion
} 