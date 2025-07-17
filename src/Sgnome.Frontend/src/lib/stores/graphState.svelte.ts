import type { Node, Edge, GraphResponse, GraphState, GraphMetadata } from '../types/graph';
import { NodeState, PinState } from '../types/graph';

// Initialize reactive state with $state
let state = $state<GraphState>({
  nodes: [],
  edges: [],
  metadata: new Map(),
  lastUpdated: ''
});

// Derived state for computed values (like NgRx selectors)
const nodeCount = $derived(state.nodes.length);
const edgeCount = $derived(state.edges.length);
const metadataCount = $derived(state.metadata.size);

// Selector functions for derived values
function selectNodeCount() {
  return nodeCount;
}

function selectEdgeCount() {
  return edgeCount;
}

function selectMetadataCount() {
  return metadataCount;
}

// Reducer-like function for complex state updates
function reduceGraph(current: GraphState, newData: Partial<GraphResponse>): GraphState {
  // Merge nodes (prevent duplicates, preserve existing, add new)
  const updatedNodes = newData.nodes
    ? [
        ...current.nodes.filter((existing: Node) => 
          !newData.nodes!.some((newNode: Node) => newNode.id === existing.id)
        ),
        ...newData.nodes
      ]
    : current.nodes;

  // Merge edges (prevent duplicates, preserve existing, add new)
  const updatedEdges = newData.edges
    ? [
        ...current.edges.filter((existing: Edge) => 
          !newData.edges!.some((newEdge: Edge) => newEdge.id === existing.id)
        ),
        ...newData.edges
      ]
    : current.edges;

  // Merge metadata
  const updatedMetadata = new Map(current.metadata);
  if (newData.metadata) {
    updatedMetadata.set(newData.metadata.queryId, newData.metadata);
  }

  return {
    nodes: updatedNodes,
    edges: updatedEdges,
    metadata: updatedMetadata,
    lastUpdated: new Date().toISOString()
  };
}

// Update state with reducer
function updateGraph(newData: Partial<GraphResponse>) {
  state = reduceGraph(state, newData);
}

// Action-like functions for user interactions
function addNode(node: Node) {
  state = { 
    ...state, 
    nodes: [...state.nodes.filter((n: Node) => n.id !== node.id), node],
    lastUpdated: new Date().toISOString()
  };
}

function removeNode(nodeId: string) {
  state = {
    ...state,
    nodes: state.nodes.filter((n: Node) => n.id !== nodeId),
    edges: state.edges.filter((e: Edge) => e.source !== nodeId && e.target !== nodeId),
    lastUpdated: new Date().toISOString()
  };
}

function addEdge(edge: Edge) {
  state = {
    ...state,
    edges: [...state.edges.filter((e: Edge) => e.id !== edge.id), edge],
    lastUpdated: new Date().toISOString()
  };
}

function removeEdge(edgeId: string) {
  state = {
    ...state,
    edges: state.edges.filter((e: Edge) => e.id !== edgeId),
    lastUpdated: new Date().toISOString()
  };
}

// Clear the entire graph
function clearGraph() {
  state = {
    nodes: [],
    edges: [],
    metadata: new Map(),
    lastUpdated: new Date().toISOString()
  };
}

// Generic API call function using pin metadata (like NgRx effects)
async function fetchFromPin(nodeId: string, pinId: string) {
  const node = state.nodes.find(n => n.id === nodeId);
  if (!node) {
    console.error('Node not found:', nodeId);
    return;
  }

  const pin = node.data.pins.find(p => p.id === pinId);
  if (!pin || !pin.metadata?.apiEndpoint) {
    console.error('Pin not found or missing API endpoint:', pinId);
    return;
  }

  try {
    // Update node state to loading
    const updatedNode = {
      ...node,
      data: { ...node.data, state: NodeState.Loading }
    };
    addNode(updatedNode);

    // Update pin state to loading
    const updatedPins = node.data.pins.map(p => 
      p.id === pinId ? { ...p, state: PinState.Loading } : p
    );
    const nodeWithUpdatedPins = {
      ...updatedNode,
      data: { ...updatedNode.data, pins: updatedPins }
    };
    addNode(nodeWithUpdatedPins);

    // Make API call using pin metadata
    const response = await fetch(pin.metadata.apiEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(pin.metadata.parameters)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const graphResponse: GraphResponse = await response.json();
    updateGraph(graphResponse);

    // Update pin state to expanded
    const finalPins = nodeWithUpdatedPins.data.pins.map(p => 
      p.id === pinId ? { ...p, state: PinState.Expanded } : p
    );
    const finalNode = {
      ...nodeWithUpdatedPins,
      data: { ...nodeWithUpdatedPins.data, state: NodeState.Loaded, pins: finalPins }
    };
    addNode(finalNode);

  } catch (error) {
    console.error('Failed to fetch from pin:', error);
    
    // Update node state to error
    const errorNode = {
      ...node,
      data: { ...node.data, state: NodeState.Error }
    };
    addNode(errorNode);

    // Update pin state to unexpanded (allow retry)
    const errorPins = node.data.pins.map(p => 
      p.id === pinId ? { ...p, state: PinState.Unexpanded } : p
    );
    const errorNodeWithPins = {
      ...errorNode,
      data: { ...errorNode.data, pins: errorPins }
    };
    addNode(errorNodeWithPins);
  }
}

// Development helper function for testing with canned endpoints
async function fetchWithEndpoint(endpoint: string, parameters: Record<string, unknown> = {}) {
  try {
    const response = await fetch(endpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(parameters)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const graphResponse: GraphResponse = await response.json();
    updateGraph(graphResponse);
  } catch (error) {
    console.error('Failed to fetch with endpoint:', error);
  }
}

// Pin expansion function
async function expandPin(nodeId: string, pinId: string) {
  const node = state.nodes.find(n => n.id === nodeId);
  if (!node) return;

  const pin = node.data.pins.find(p => p.id === pinId);
  if (!pin || pin.type !== 'expandable' || !pin.metadata?.apiEndpoint) return;

  try {
    const response = await fetch(pin.metadata.apiEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        nodeId,
        pinId,
        ...pin.metadata.parameters
      })
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const graphResponse: GraphResponse = await response.json();
    updateGraph(graphResponse);
  } catch (error) {
    console.error('Failed to expand pin:', error);
  }
}

// Get node by ID
function getNode(nodeId: string): Node | undefined {
  return state.nodes.find(n => n.id === nodeId);
}

// Get edges for a node
function getNodeEdges(nodeId: string): Edge[] {
  return state.edges.filter(e => e.source === nodeId || e.target === nodeId);
}

// Get metadata for an operation
function getMetadata(queryId: string): GraphMetadata | undefined {
  return state.metadata.get(queryId);
}

// Export a getter function for the state
function getState() {
  return state;
}

export { 
  getState, 
  selectNodeCount, 
  selectEdgeCount, 
  selectMetadataCount,
  updateGraph, 
  fetchFromPin,
  fetchWithEndpoint,
  expandPin,
  addNode, 
  removeNode,
  addEdge,
  removeEdge,
  clearGraph,
  getNode,
  getNodeEdges,
  getMetadata
}; 