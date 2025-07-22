# Sgnome Architecture Documentation

## Overview

Sgnome is a gaming research tool that allows users to explore relationships between games, developers, and publishers through an interactive graph interface. The system is designed around a domain-driven service architecture that provides both node resolution and cross-domain data enrichment through a unified "Consume" pattern.

## Core Design Principles

### 1. Domain-Driven Services
Services are organized by domain, each owning a specific node type and providing both node resolution and cross-domain pin generation:
- **PlayerService** - Owns `PlayerNode`, provides player-centric information and cross-domain links
- **LibraryService** - Owns `LibraryNode` and `LibraryListNode`, provides library-centric information and cross-domain links
- **GamesService** - Owns `GameNode`, provides game-centric information and cross-domain links

**Key Principle**: **Services follow a unified "Consume" pattern where self-domain consumption returns resolved nodes and foreign-domain consumption returns pins for navigation and data enrichment**

### 2. Consume Pattern
Each service implements a consistent interface that handles both node resolution and cross-domain data generation:

```csharp
public interface ILibraryService
{
    // Self-domain: Return resolved node + pins for data enrichment
    Task<(IEnumerable<Pin> Pins, LibraryNode ResolvedNode)> Consume(LibraryNode partial);
    Task<(IEnumerable<Pin> Pins, LibraryListNode ResolvedNode)> Consume(LibraryListNode partial);
    
    // Foreign-domain: Return pins for navigation and data enrichment
    Task<IEnumerable<Pin>> Consume(PlayerNode player);
}
```

### 3. Pin Generator Pattern
Pin generation is separated into pure functions that convert domain nodes to pins:

```
Service/
├── Database/              # Redis-first persistence layer
│   ├── ILibraryDatabase   # Node resolution interface
│   └── RedisLibraryDatabase # Get/Create implementation
├── PinGenerators/         # Pure functions for pin generation
│   ├── LibraryPinGenerator # Converts LibraryNode to pins
│   └── LibraryListPinGenerator # Converts LibraryListNode to pins
└── Service                # Orchestrates resolution and pin generation
```

### 4. Redis-First Database Architecture
The system uses Redis as the primary database with a "Get/Create" (MegaUpsert) pattern:
- **Node Resolution**: `ResolveNodeAsync()` either retrieves existing nodes or creates new ones
- **Identifier Management**: `AddIdentifiersAsync()` and `AddLibrarySourceMappingsAsync()` for relationship building
- **Multi-Identifier Lookup**: Nodes can be resolved by multiple external identifiers (Steam ID, Epic ID, etc.)

## Service Architecture

### Consume Pattern Implementation
Services implement a unified interface that handles both self-domain and foreign-domain operations:

#### Self-Domain Consumption
When consuming nodes of the service's own domain, the service:
1. Resolves the node (Get/Create pattern)
2. Generates pins for data enrichment (API calls, informational pins)
3. Returns both the resolved node and enrichment pins

```csharp
public async Task<(IEnumerable<Pin> Pins, LibraryNode ResolvedNode)> Consume(LibraryNode partial)
{
    var resolvedNode = await _libraryDatabase.ResolveLibraryAsync(
        partial.Identifiers, 
        partial.LibrarySource, 
        partial.DisplayName
    );
    
    var pins = await CreateLibraryPinsAsync(resolvedNode, resolvedNode.InternalId!, "library");
    return (pins, resolvedNode);
}
```

#### Foreign-Domain Consumption
When consuming nodes from other domains, the service:
1. Analyzes the foreign node for relevant data
2. Resolves related nodes in its domain
3. Generates pins for navigation and data enrichment
4. Returns only pins (no resolved node)

```csharp
public async Task<IEnumerable<Pin>> Consume(PlayerNode player)
{
    var allPins = new List<Pin>();
    
    // Resolve libraries for this player
    var resolvedLibraries = new List<LibraryNode>();
    foreach (var (source, identifier) in player.Identifiers)
    {
        if (source != PlayerIdentifiers.Internal)
        {
            var library = await _libraryDatabase.ResolveLibraryAsync(
                new Dictionary<string, string> { [source] = identifier },
                source,
                $"{source} Library"
            );
            resolvedLibraries.Add(library);
        }
    }
    
    // Generate pins for each library
    foreach (var library in resolvedLibraries)
    {
        var libraryPins = await CreateLibraryPinsAsync(library, player.InternalId, "player");
        allPins.AddRange(libraryPins);
    }
    
    // Generate collection summary pin
    if (resolvedLibraries.Count > 0)
    {
        var libraryList = await _libraryListDatabase.ResolveLibraryListAsync(player.InternalId, "Game Libraries");
        var librarySourceMappings = resolvedLibraries.ToDictionary(lib => lib.LibrarySource, lib => lib.InternalId!);
        await _libraryListDatabase.AddLibrarySourceMappingsAsync(libraryList.InternalId!, librarySourceMappings);
        
        var updatedLibraryList = await _libraryListDatabase.ResolveLibraryListAsync(player.InternalId, "Game Libraries");
        var collectionPins = await CreateLibraryListPinsAsync(updatedLibraryList, player.InternalId, "player");
        allPins.AddRange(collectionPins);
    }
    
    return allPins;
}
```

### Pin Generator Pattern
Pin Generators are pure functions that convert domain nodes to pins, with context provided by the service:

```csharp
public class LibraryPinGenerator
{
    private readonly ISteamLibraryProvider _steamProvider;
    
    public async Task<IEnumerable<Pin>> GeneratePinsAsync(LibraryNode library, PinContext context)
    {
        var pins = new List<Pin>();
        
        // Generate informational pins about the library
        if (library.LibrarySource == "steam")
        {
            var steamPins = await _steamProvider.GetLibraryPinsAsync(library.Identifiers["steam"], context);
            pins.AddRange(steamPins);
        }
        
        return pins;
    }
}
```

### Database Pattern
Databases implement a "Get/Create" (MegaUpsert) pattern that ensures data is always up-to-date:

```csharp
public interface ILibraryDatabase
{
    Task<LibraryNode> ResolveLibraryAsync(Dictionary<string, string> identifiers, string librarySource, string? displayName = null);
    Task AddIdentifiersAsync(string internalId, Dictionary<string, string> identifiers);
    Task<LibraryNode?> GetByInternalIdAsync(string internalId);
}
```

## Data Flow Architecture

### Request Flow
1. **Controller** receives API request with partial node data
2. **Service** calls `Consume()` with the partial node
3. **Database** resolves or creates the node using "Get/Create" pattern
4. **Pin Generators** convert nodes to pins with context
5. **Service** returns resolved node and/or pins
6. **Controller** builds `GraphResponse` with nodes, edges, and pins

### Cross-Domain Flow
1. **Controller** calls foreign domain service's `Consume()` method
2. **Service** analyzes foreign node and resolves related nodes in its domain
3. **Pin Generators** create navigation and enrichment pins
4. **Service** returns pins for the foreign domain
5. **Controller** aggregates pins from multiple services

## Graph Data Model

### Node Structure
```json
{
  "id": "player-76561198000000000",
  "type": "default",
  "x": 100, "y": 100,
  "data": {
    "label": "Alex",
    "nodeType": "player",
    "properties": {
      "internalId": "guid-123",
      "identifiers": {
        "steam": "76561198000000000",
        "epic": "epic-user-123"
      }
    },
    "pins": [],
    "state": "loaded"
  }
}
```

### Pin Structure
```json
{
  "id": "steam-library",
  "label": "Steam Library",
  "type": "library",
  "state": "unexpanded",
  "behavior": "expandable",
  "summary": {
    "displayText": "150 games",
    "count": 150,
    "icon": "library"
  },
  "metadata": {
    "targetNodeType": "library",
    "targetNodeId": "library-guid-456",
    "originNodeId": "player-76561198000000000",
    "apiEndpoint": "/api/library/select",
    "parameters": {
      "internalId": "library-guid-456"
    }
  }
}
```

## Database Architecture

### Redis-First Design
- **Primary Database**: Redis stores all node data and relationships
- **Multi-Identifier Resolution**: Nodes can be found by any external identifier
- **Internal ID Generation**: All nodes have a reliable internal identifier
- **Incremental Building**: Relationships are built incrementally as data is discovered

### Key Patterns
- **Get/Create (MegaUpsert)**: `ResolveNodeAsync()` ensures data is always up-to-date
- **Identifier Management**: `AddIdentifiersAsync()` and `AddLibrarySourceMappingsAsync()` for relationship building
- **Reverse Indexing**: Efficient lookup of relationships in both directions

## Controller Orchestration

Controllers are responsible for:
- Calling the appropriate service's `Consume()` method
- Aggregating pins from multiple services
- Building the `GraphResponse` with nodes, edges, and pins

```csharp
public async Task<GraphResponse> SelectPlayer(PlayerNode player)
{
    var resolvedPlayer = await _playerService.Consume(player);
    var libraryPins = await _libraryService.Consume(resolvedPlayer.ResolvedNode);
    
    var allPins = new List<Pin>();
    allPins.AddRange(resolvedPlayer.Pins);
    allPins.AddRange(libraryPins);
    
    return new GraphResponse
    {
        Nodes = new[] { resolvedPlayer.ResolvedNode },
        Edges = new Edge[] { },
        Pins = allPins
    };
}
```

## Pin Generation Strategy

### Pin Types
1. **Navigation Pins** - Expand to create new nodes and edges
2. **Informational Pins** - Display data inline without expansion
3. **Collection Pins** - Summarize collections of related nodes
4. **Analytics Pins** - Show computed data and statistics

### Pin Context
Each pin includes context about its origin and target:
- **Origin Node ID** - The node that owns this pin
- **Target Node Type** - What type of node this pin expands to
- **Target Node ID** - The specific node this pin targets (if known)
- **API Endpoint** - Where to fetch expansion data
- **Parameters** - Context for the API call

## Extensibility

### Adding New Services
1. Create domain node class (e.g., `PublisherNode`)
2. Implement database interface with "Get/Create" pattern
3. Create Pin Generators for the domain
4. Implement service with `Consume()` methods
5. Add controller for API endpoints

### Adding New Pin Types
1. Define pin behavior and metadata structure
2. Create Pin Generator for the new pin type
3. Update service to use the new Pin Generator
4. Add frontend support for the new pin type

## Performance Considerations

### Caching Strategy
- **Node Resolution**: Redis-first design provides automatic caching
- **Pin Generation**: Pin Generators can implement their own caching
- **API Calls**: Providers handle external API caching and rate limiting

### Lazy Loading
- Nodes load with minimal data initially
- Pins provide preview information
- Full data loads when pins are expanded
- Progressive disclosure reduces initial load time

## Security Considerations

### API Key Management
- Provider API keys stored securely in configuration
- No keys exposed to frontend
- Rate limiting per provider
- Key rotation support
