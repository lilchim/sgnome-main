# Database Architecture and Redis Patterns

## Overview

Sgnome uses Redis as both a database and cache layer, implementing a Redis-first architecture with multi-identifier resolution and reverse indexing patterns.

## Core Database Principles

### 1. Redis-First Design
- **Primary Storage**: Redis is the primary database, not just a cache
- **Multi-Identifier Resolution**: Nodes can be resolved by multiple identifier types
- **Reverse Indexing**: Efficient lookup of relationships in both directions
- **Internal ID Generation**: All nodes have reliable internal identifiers

### 2. Node Storage Pattern

#### Basic Node Storage
```redis
# Main node data
{nodeType}:{internalId} → JSON serialized node

# Identifier lookups
{nodeType}:{identifierType}:{identifierValue} → {internalId}

# Relationship lookups
{nodeType}:{relationshipType}:{relationshipValue} → {internalId}
```

#### Example: PlayerNode Storage
```redis
# Main player data
player:player-abc123 → {"InternalId":"player-abc123","DisplayName":"Alex",...}

# Identifier lookups
player:steam:76561198012345678 → player-abc123
player:epic:epic123456 → player-abc123

# Relationship lookups
player:displayname:Alex → player-abc123
```

### 3. Multi-Identifier Resolution

Each node type supports resolution by multiple identifier types:

#### PlayerNode Identifiers
```csharp
public Dictionary<string, string> Identifiers { get; set; } = new()
{
    ["steam"] = "76561198012345678",
    ["epic"] = "epic123456",
    ["displayname"] = "Alex"
};
```

#### LibraryNode Identifiers
```csharp
public Dictionary<string, string> Identifiers { get; set; } = new()
{
    ["player"] = "player-abc123",
    ["steam"] = "76561198012345678"
};
```

#### Resolution Strategy
1. **Try InternalId**: Direct lookup if internal ID is provided
2. **Try Identifiers**: Lookup by each identifier type
3. **Create New**: Generate new internal ID and store if not found

### 4. Reverse Indexing Pattern

For efficient relationship lookups, we maintain reverse indexes:

#### Collection Membership Index
```redis
# Library → Collections reverse index
library:{libraryInternalId}:collections → Hash of {librariesInternalId: "1"}

# Example
library:lib-123:collections → {"liblist-456": "1", "liblist-789": "1"}
```

#### Usage Pattern
```csharp
// Find all collections containing a library
var collectionIds = await db.HashKeysAsync($"library:{libraryInternalId}:collections");

// Add library to collection
await db.HashSetAsync($"library:{libraryInternalId}:collections", librariesInternalId, "1");

// Remove library from collection
await db.HashDeleteAsync($"library:{libraryInternalId}:collections", librariesInternalId);
```

### 5. Collection Mapping Pattern

Collections use source-to-member mapping for efficient navigation:

#### LibraryListNode Storage
```csharp
public Dictionary<string, string> LibrarySourceMapping { get; set; } = new()
{
    ["steam"] = "library-123",
    ["epic"] = "library-456"
};
```

#### Benefits
- **Efficient Navigation**: Direct lookup of members by source
- **No Back-References**: Members don't need to know about collections
- **Flexible Membership**: Members can belong to multiple collections
- **Clean Separation**: Collections manage their members, not vice versa

## Database Interface Pattern

### Standard Database Interface
```csharp
public interface INodeDatabase<T> where T : class
{
    // Core operations
    Task StoreAsync(T node);
    Task<T?> GetByInternalIdAsync(string internalId);
    Task<T?> GetByIdentifierAsync(string identifierType, string identifierValue);
    
    // Relationship operations
    Task<IEnumerable<T>> GetByRelationshipAsync(string relationshipType, string relationshipValue);
    Task UpdateIdentifiersAsync(string internalId, Dictionary<string, string> identifiers);
    
    // Reverse indexing (if applicable)
    Task AddReverseIndexAsync(string nodeInternalId, string relatedInternalId);
    Task RemoveReverseIndexAsync(string nodeInternalId, string relatedInternalId);
    Task<IEnumerable<string>> GetReverseIndexAsync(string nodeInternalId);
}
```

### Implementation Example: RedisPlayerDatabase
```csharp
public class RedisPlayerDatabase : IPlayerDatabase
{
    public async Task StoreAsync(PlayerNode player)
    {
        // Generate internal ID if not provided
        if (string.IsNullOrEmpty(player.InternalId))
        {
            player.InternalId = $"player-{Guid.NewGuid():N}";
        }

        // Store main node data
        var nodeKey = $"player:{player.InternalId}";
        var nodeJson = JsonSerializer.Serialize(player);
        await db.StringSetAsync(nodeKey, nodeJson);

        // Store identifier lookups
        foreach (var (identifierType, identifierValue) in player.Identifiers)
        {
            var lookupKey = $"player:{identifierType}:{identifierValue}";
            await db.StringSetAsync(lookupKey, player.InternalId);
        }
    }

    public async Task<PlayerNode?> GetByIdentifierAsync(string identifierType, string identifierValue)
    {
        var lookupKey = $"player:{identifierType}:{identifierValue}";
        var internalId = await db.StringGetAsync(lookupKey);
        
        if (internalId.IsNull) return null;
        return await GetByInternalIdAsync(internalId!);
    }
}
```

## Data-Driven vs Provider-Driven Architecture

### Provider-Driven (Old Pattern)
```csharp
// Check what providers we have, then look for player IDs
if (!string.IsNullOrEmpty(player.SteamId))
{
    var steamPins = await _steamProvider.GetLibraryPinsAsync(player.SteamId, context);
    pins.AddRange(steamPins);
}
```

### Data-Driven (New Pattern)
```csharp
// Find what libraries the player actually has
var playerLibraries = await _libraryDatabase.GetByPlayerIdAsync(player.InternalId);
foreach (var library in playerLibraries)
{
    var pins = await GetLibraryInfoFromProvider(library, context);
    pins.AddRange(pins);
}
```

### Benefits of Data-Driven Approach
- **Redis-Leveraged**: Uses database to find actual relationships
- **Provider-Independent**: Works regardless of provider availability
- **Extensible**: Easy to add new providers without changing core logic
- **Robust**: Only shows data the user actually has

## Key Design Decisions

### 1. No Back-References
- **Collections manage members**: LibraryListNode knows about LibraryNodes
- **Members don't know collections**: LibraryNode doesn't know about LibraryListNode
- **Reverse indexing for lookups**: Use Redis hashes for efficient reverse lookups
- **Clean separation**: Prevents circular dependencies and complex relationship management

### 2. Internal ID Generation
- **Consistent format**: `{nodeType}-{guid}` (e.g., `player-abc123`)
- **Reliable identification**: Always available for relationships
- **Fallback chain**: Use InternalId → SteamId → EpicId → DisplayName for logging
- **Database independence**: Internal IDs work regardless of external service availability

### 3. Identifier Mapping
- **Flexible relationships**: Store any identifier type in the dictionary
- **Consistent pattern**: All node types use the same identifier mapping approach
- **Extensible**: Easy to add new identifier types without changing the model
- **Redis-friendly**: All lookups use the same pattern

### 4. Collection Design
- **Source mapping**: Collections map sources to member internal IDs
- **Efficient navigation**: Direct lookup of members by source
- **Multiple collections**: Members can belong to multiple collections
- **No duplication**: Single source of truth for member data

## Performance Considerations

### Redis Key Design
- **Consistent naming**: `{nodeType}:{identifierType}:{value}`
- **Efficient lookups**: Direct key access for identifier resolution
- **Hash operations**: Use Redis hashes for reverse indexing
- **Memory usage**: Consider key expiration for temporary data

### Caching Strategy
- **Node data**: Cache by internal ID for frequently accessed nodes
- **Identifier lookups**: Cache identifier-to-internalId mappings
- **Collection data**: Cache collection membership for active collections
- **Provider data**: Cache external API responses separately

### Scalability
- **Horizontal scaling**: Redis cluster support for large datasets
- **Key distribution**: Consistent hashing for key distribution
- **Memory management**: Monitor memory usage and implement cleanup strategies
- **Performance monitoring**: Track lookup times and optimize slow queries

## Migration and Evolution

### Adding New Node Types
1. **Create node model** with InternalId and Identifiers properties
2. **Implement database interface** following the standard pattern
3. **Add Redis implementation** with proper key naming
4. **Update service layer** to use database for resolution

### Adding New Identifier Types
1. **Add to Identifiers dictionary** in the node model
2. **Update database implementation** to handle new lookup keys
3. **No service changes needed** - pattern is already established

### Adding New Relationships
1. **Design reverse indexing strategy** for the relationship
2. **Implement reverse index methods** in database interface
3. **Update service methods** to use reverse indexing for lookups
4. **Maintain consistency** with existing relationship patterns

## Best Practices

### 1. Always Use Internal IDs for Relationships
```csharp
// Good: Use internal IDs
public string PlayerId { get; set; } = string.Empty; // Internal ID

// Bad: Use external identifiers
public string SteamId { get; set; } = string.Empty; // External ID
```

### 2. Implement Proper Error Handling
```csharp
public async Task<T?> GetByIdentifierAsync(string identifierType, string identifierValue)
{
    try
    {
        var lookupKey = $"{nodeType}:{identifierType}:{identifierValue}";
        var internalId = await db.StringGetAsync(lookupKey);
        
        if (internalId.IsNull) return null;
        return await GetByInternalIdAsync(internalId!);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error looking up {NodeType} by {IdentifierType}: {Value}", 
            nodeType, identifierType, identifierValue);
        return null;
    }
}
```

### 3. Use Consistent Logging
```csharp
// Extract reliable identifier for logging
var playerId = player.InternalId ?? player.SteamId ?? player.EpicId ?? player.DisplayName ?? "unknown";
_logger.LogInformation("Processing player {PlayerId}", playerId);
```

### 4. Maintain Reverse Indexes
```csharp
// Always update reverse indexes when relationships change
await _libraryDatabase.AddCollectionIndexAsync(libraryInternalId, librariesInternalId);
await _libraryDatabase.RemoveCollectionIndexAsync(libraryInternalId, oldLibrariesInternalId);
```

This database architecture provides a solid foundation for Sgnome's graph-based data model while maintaining flexibility for future extensions and optimizations. 