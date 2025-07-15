# Sgnome Architecture Documentation

## Overview

Sgnome is a gaming research tool that allows users to explore relationships between games, developers, and publishers through an interactive graph interface. The system is designed around a business-focused service architecture that answers questions rather than exposing rigid APIs.

## Core Design Principles

### 1. Business-Focused Services
Services are organized by business questions, not technical concerns:
- **UserLibraryService** - Answers: "What games does user X own/play?"
- **GameInfoService** - Answers: "What are the details for game X?"
- **UserProfileService** - Answers: "What's user X's gaming profile?"

### 2. Provider-Aggregator Pattern
Each service follows a consistent pattern:
```
Service/
├── Providers/           # Data source implementations
│   ├── SteamProvider    # Steam API integration
│   ├── EpicProvider     # Epic Games Store integration
│   └── RawgProvider     # RAWG database integration
├── Aggregator           # Combines data from multiple providers
└── Service              # Thin wrapper exposing business methods
```

### 3. Graph-First Data Model
The system is designed around the concept of nodes and edges that can be visualized in a graph:
- **Nodes** represent entities (players, games, publishers)
- **Edges** represent relationships (owns, published-by, similar-to)
- **Pins** represent expandable or informational data on nodes

## Data Flow Architecture

### Frontend → Backend
1. User selects a node or searches for an entity
2. Frontend sends node data to backend
3. Backend services analyze the node and generate related data
4. Backend returns a `GraphResponse` with nodes, edges, and pins

### Backend → Frontend
1. Services receive node data and identify relevant providers
2. Providers fetch data from external APIs (Steam, Epic, RAWG)
3. Aggregators combine and transform the data
4. Services generate pins and edges based on relationships
5. `GraphResponse` is returned with complete graph structure

## Graph Data Model

### Node Structure
```json
{
  "id": "player-76561198000000000",
  "type": "default",           // xyflow node type
  "x": 100, "y": 100,         // xyflow coordinates
  "data": {                   // Our custom data
    "label": "Alex",
    "nodeType": "player",     // Our node classification
    "properties": {},         // Serialized domain object
    "pins": [],              // Expandable relationships
    "state": "loaded"        // Loading state
  }
}
```

### Pin System
Pins represent expandable relationships or informational data:

#### Expandable Pins
- Can be clicked to create new nodes/edges
- Contain metadata for API expansion
- Example: "Recently Played (5 games)" → expands to game nodes

#### Informational Pins
- Display supplementary data inline
- No expansion capability
- Example: "Released: 2020-12-10"

### Edge Structure
```json
{
  "id": "edge-1",
  "source": "player-76561198000000000",
  "target": "game-730",
  "type": "default",          // xyflow edge type
  "data": {                   // Our custom data
    "label": "Owns",
    "edgeType": "owns",       // Our relationship classification
    "properties": {}          // Relationship metadata
  }
}
```

## Service Architecture

### Provider Pattern
Providers handle integration with external data sources:

```csharp
public interface ISteamUserLibraryProvider
{
    Task<SteamUserLibraryData?> GetUserLibraryAsync(string steamId);
    Task<SteamUserLibraryData?> GetRecentlyPlayedGamesAsync(string steamId);
}
```

### Aggregator Pattern
Aggregators combine data from multiple providers and generate pins:

```csharp
public class UserLibraryAggregator
{
    private readonly ISteamUserLibraryProvider _steamProvider;
    private readonly IEpicUserLibraryProvider _epicProvider;
    
    public async Task<GraphResponse> GetUserLibraryAsync(PlayerNode player)
    {
        // Combine data from multiple providers
        // Generate pins for expandable relationships
        // Create edges between related nodes
    }
}
```

### Service Layer
Services provide business-focused APIs:

```csharp
public interface IUserLibraryService
{
    Task<GraphResponse> GetUserLibraryAsync(PlayerNode player);
    Task<GraphResponse> GetRecentlyPlayedGamesAsync(PlayerNode player);
}
```

## Pin Generation Strategy

### Pin Types
1. **Relationship Pins** - Connect to other entities (publisher, similar games)
2. **Collection Pins** - Expand to lists (recently played, owned games)
3. **Analytics Pins** - Show computed data (favorite genres, playtime stats)
4. **Informational Pins** - Display facts (release date, player count)

### Pin Metadata
Each expandable pin contains:
- **Target Node Type** - What type of node will be created
- **API Endpoint** - Where to fetch the expansion data
- **Parameters** - Additional context for the API call

### Response Metadata
Each `GraphResponse` includes metadata for state management:

```json
{
  "metadata": {
    "operationId": "user-library-76561198000000000",
    "operationType": "GetUserLibrary",
    "sourceNodeId": "player-76561198000000000",
    "timestamp": "2024-01-15T10:30:00Z",
    "provider": "Steam",
    "expandedPins": ["recently-played", "owned-games"],
    "nodeCount": 15,
    "edgeCount": 12
  }
}
```

#### Metadata Fields
- **operationId** - Unique identifier for this specific operation
- **operationType** - The service method that was called
- **sourceNodeId** - The node that triggered this response
- **timestamp** - When the data was generated
- **provider** - Primary data source used
- **expandedPins** - Which pins were expanded in this response
- **nodeCount/edgeCount** - For debugging and optimization

## Frontend Integration

### xyflow Compatibility
The system is designed to work seamlessly with xyflow:
- Nodes and edges follow xyflow's expected structure
- Custom data is stored in the `data` field
- Frontend can directly pass nodes/edges to xyflow

### Pin Interaction
1. User clicks an expandable pin
2. Frontend extracts API endpoint from pin metadata
3. Frontend makes API call to expand the relationship
4. Backend returns new nodes/edges with metadata
5. Frontend merges response into existing graph state
6. Frontend updates xyflow with merged state

### Graph State Management
The frontend is responsible for maintaining a unified graph state:

#### Responsibilities
- **State Aggregation** - Merge multiple API responses into single graph
- **Duplicate Prevention** - Use node/edge IDs to prevent duplicates
- **Conflict Resolution** - Use metadata timestamps for precedence
- **Pin State Tracking** - Track which pins have been expanded
- **Loading States** - Show loading indicators during API calls

#### Implementation Considerations
- **Memory Management** - Limit graph size, implement cleanup strategies
- **Performance** - Efficient merging algorithms for large graphs
- **User Experience** - Smooth transitions, loading states, error handling
- **Debugging** - Log metadata for troubleshooting

### State Management
- **Node State** - Tracks loading/loaded/error states
- **Pin State** - Tracks unexpanded/loading/expanded states
- **Graph Context** - Maintains query history and context

### Graph State Aggregation
The frontend maintains a unified graph state that gets incrementally updated:

#### Metadata-Driven Updates
- Each `GraphResponse` includes metadata identifying the source operation
- Frontend uses metadata to determine how to merge new data with existing state
- Prevents duplicate nodes/edges while preserving existing relationships

#### State Aggregation Logic
```typescript
interface GraphState {
  nodes: Map<string, Node>;           // Keyed by node ID
  edges: Map<string, Edge>;           // Keyed by edge ID
  metadata: Map<string, ResponseMetadata>; // Track response sources
}

function mergeGraphResponse(
  currentState: GraphState, 
  response: GraphResponse
): GraphState {
  // Merge nodes (preserve existing, add new, update if newer)
  // Merge edges (prevent duplicates, update if newer)
  // Track metadata for debugging/optimization
  // Handle conflicts based on metadata timestamps
}
```

#### Update Strategies
1. **Node Updates** - Merge properties, preserve existing pins, add new pins
2. **Edge Updates** - Prevent duplicates, update properties if newer
3. **Pin Expansion** - Replace pin with expanded nodes/edges, update source node
4. **Conflict Resolution** - Use metadata timestamps to determine precedence

## Extensibility

### Adding New Node Types
1. Create domain class (e.g., `DeveloperNode`)
2. Add to `NodeBuilder` for xyflow conversion
3. Create corresponding service and providers
4. Update pin generation logic

### Adding New Data Sources
1. Create provider interface and implementation
2. Add provider to aggregator
3. Update pin generation to use new data
4. No changes needed to existing services

### Adding New Pin Types
1. Define pin behavior (expandable/informational)
2. Create pin generation logic in aggregators
3. Update frontend to handle new pin types
4. Add API endpoints for expansion

## Performance Considerations

### Caching Strategy
- **Node Data** - Cache by node ID and type
- **Pin Data** - Cache by pin type and context
- **Provider Data** - Cache external API responses
- **Graph Context** - Cache query results

### Lazy Loading
- Nodes load with minimal data initially
- Pins provide preview information
- Full data loads only when pins are expanded
- Progressive disclosure reduces initial load time

### Streaming Support
- HTTP/2 server-sent events for real-time updates
- Progressive pin population
- Live data updates (player counts, etc.)

## Security Considerations

### API Key Management
- Provider API keys stored securely
- No keys exposed to frontend
- Rate limiting per provider
- Key rotation support

### Data Privacy
- User consent for data access
- Minimal data collection
- Secure data transmission
- GDPR compliance considerations 