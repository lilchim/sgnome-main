using StackExchange.Redis;
using Sgnome.Models.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LibraryService.Database;

/// <summary>
/// Redis implementation of library node persistence with Get/Update/Create functionality
/// </summary>
public class RedisLibraryDatabase : ILibraryDatabase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisLibraryDatabase> _logger;

    public RedisLibraryDatabase(
        IConnectionMultiplexer redis,
        ILogger<RedisLibraryDatabase> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <summary>
    /// MegaUpsert: Get existing library, update with new identifiers, or create new library
    /// </summary>
    public async Task<LibraryNode> ResolveLibraryAsync(Dictionary<string, string> identifiers, string librarySource, string? displayName = null)
    {
        _logger.LogDebug("Resolving library with {IdentifierCount} identifiers for {LibrarySource}", 
            identifiers.Count, librarySource);

        // GET: Try to find existing library by any identifier
        var existingLibrary = await FindExistingLibraryAsync(identifiers, librarySource);
        
        if (existingLibrary != null)
        {
            // Found existing library - return as-is (libraries are created with complete identifier sets)
            _logger.LogDebug("Found existing library {InternalId} for {LibrarySource}", existingLibrary.InternalId, librarySource);
            return existingLibrary;
        }

        // CREATE: Generate new library with all provided identifiers
        _logger.LogDebug("Creating new library for {LibrarySource}", librarySource);
        var newLibrary = new LibraryNode
        {
            LibrarySource = librarySource,
            DisplayName = displayName ?? $"{librarySource} Library",
            Identifiers = new Dictionary<string, string>(identifiers), // Copy all identifiers
            AvailableCategories = new List<string> { "recently-played", "owned-games" },
            LastUpdated = DateTime.UtcNow
        };

        // Generate internal ID and store
        newLibrary.InternalId = $"library-{Guid.NewGuid():N}";
        await StoreLibraryAsync(newLibrary);

        _logger.LogInformation("Created new library {InternalId} for {LibrarySource}", 
            newLibrary.InternalId, librarySource);
        
        return newLibrary;
    }

    /// <summary>
    /// Adds additional identifiers to an existing library
    /// </summary>
    public async Task AddIdentifiersAsync(string internalId, Dictionary<string, string> identifiers)
    {
        var db = _redis.GetDatabase();
        var nodeKey = $"library:{internalId}";
        
        var nodeJson = await db.StringGetAsync(nodeKey);
        if (nodeJson.IsNull)
        {
            throw new InvalidOperationException($"LibraryNode {internalId} not found");
        }

        var library = JsonSerializer.Deserialize<LibraryNode>(nodeJson!)!;
        
        // Add new identifiers
        foreach (var (identifierType, identifierValue) in identifiers)
        {
            if (!library.Identifiers.ContainsKey(identifierType))
            {
                library.Identifiers[identifierType] = identifierValue;
                
                // Add lookup entry
                var lookupKey = $"library:{identifierType}:{identifierValue}";
                await db.StringSetAsync(lookupKey, internalId);
            }
        }

        library.LastUpdated = DateTime.UtcNow;

        // Store updated node
        var updatedJson = JsonSerializer.Serialize(library);
        await db.StringSetAsync(nodeKey, updatedJson);

        _logger.LogInformation("Added {IdentifierCount} identifiers to library {InternalId}", 
            identifiers.Count, internalId);
    }

    /// <summary>
    /// Gets a library by its internal ID
    /// </summary>
    public async Task<LibraryNode?> GetByInternalIdAsync(string internalId)
    {
        return await GetLibraryByInternalIdAsync(internalId);
    }

    #region Private Helper Methods

    private async Task<LibraryNode?> FindExistingLibraryAsync(Dictionary<string, string> identifiers, string librarySource)
    {
        var db = _redis.GetDatabase();

        // Try each identifier to find existing library
        foreach (var (identifierType, identifierValue) in identifiers)
        {
            var lookupKey = $"library:{identifierType}:{identifierValue}";
            var internalId = await db.StringGetAsync(lookupKey);

            if (!internalId.IsNull)
            {
                _logger.LogDebug("Found library via {IdentifierType}: {IdentifierValue} -> {InternalId}",
                    identifierType, identifierValue, internalId);
                
                return await GetLibraryByInternalIdAsync(internalId!);
            }
        }

        // Special case: If we have player ID and library source, try to find by that combination
        if (identifiers.TryGetValue("player", out var playerId))
        {
            var lookupKey = $"library:player:{playerId}";
            var internalId = await db.StringGetAsync(lookupKey);
            
            if (!internalId.IsNull)
            {
                var library = await GetLibraryByInternalIdAsync(internalId!);
                if (library?.LibrarySource == librarySource)
                {
                    _logger.LogDebug("Found library via player {PlayerId} and source {Source} -> {InternalId}",
                        playerId, librarySource, library.InternalId);
                    return library;
                }
            }
        }

        return null;
    }



    private async Task StoreLibraryAsync(LibraryNode library)
    {
        var db = _redis.GetDatabase();
        
        // Store the main node data
        var nodeKey = $"library:{library.InternalId}";
        var nodeJson = JsonSerializer.Serialize(library);
        await db.StringSetAsync(nodeKey, nodeJson);

        // Store identifier lookups
        foreach (var (identifierType, identifierValue) in library.Identifiers)
        {
            var lookupKey = $"library:{identifierType}:{identifierValue}";
            await db.StringSetAsync(lookupKey, library.InternalId);
        }
    }

    private async Task<LibraryNode?> GetLibraryByInternalIdAsync(string internalId)
    {
        var db = _redis.GetDatabase();
        var nodeKey = $"library:{internalId}";
        
        var nodeJson = await db.StringGetAsync(nodeKey);
        if (nodeJson.IsNull)
        {
            return null;
        }

        return JsonSerializer.Deserialize<LibraryNode>(nodeJson!);
    }

    #endregion
} 