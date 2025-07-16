# Sgnome Frontend

A Svelte 5 application using runes for state management and xyflow for graph visualization.

## Architecture

This frontend follows a modern Svelte 5 pattern using runes for state management, avoiding the older Svelte 3/4 store patterns.

### State Management Pattern

We use a module-based state management approach with Svelte 5 runes:

- **`$state`** - For reactive state variables
- **`$derived`** - For computed values (like NgRx selectors)
- **`.svelte.ts` modules** - For centralized state logic
- **Immutable updates** - Always update state with new objects to trigger reactivity

### File Structure

```
src/
├── lib/
│   ├── types/
│   │   └── graph.ts              # TypeScript interfaces matching C# models
│   ├── stores/
│   │   └── graphState.svelte.ts  # Main state management module
│   └── components/
│       └── GraphView.svelte      # Main graph visualization component
├── App.svelte                    # Root component
└── main.ts                       # Application entry point
```

### Key Features

1. **Graph State Management** - Centralized state for nodes, edges, and metadata
2. **API Integration** - Functions for fetching and merging graph data
3. **Pin Expansion** - Support for expandable relationships
4. **Reactive Updates** - Automatic UI updates when state changes
5. **Type Safety** - Full TypeScript support with interfaces matching backend models

### Usage

The main component `GraphView.svelte` demonstrates:

- Adding test nodes to the graph
- Fetching user library data from the API
- Clearing the graph state
- Displaying reactive statistics

### Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Type check
npm run check

# Build for production
npm run build
```

### State Module Pattern

The `graphState.svelte.ts` module provides:

- **Reactive State**: `state` with nodes, edges, and metadata
- **Derived Values**: `nodeCount`, `edgeCount`, `metadataCount`
- **Actions**: `addNode`, `removeNode`, `addEdge`, `removeEdge`
- **API Functions**: `fetchUserLibrary`, `expandPin`
- **Utility Functions**: `getNode`, `getNodeEdges`, `getMetadata`

### Integration with Backend

The frontend expects API responses in the format defined by the C# `GraphResponse` model:

```typescript
interface GraphResponse {
  nodes: Node[];
  edges: Edge[];
  metadata: ResponseMetadata;
}
```

This matches the backend's graph data structure and enables seamless integration.
