import type { Node, Edge, GraphResponse, GraphState, GraphMetadata } from '../types/graph';
import { NodeState, PinState } from '../types/graph';

let state = $state<GraphState>({
  nodes: [],
  edges: [],
  metadata: new Map(),
  lastUpdated: ''
});

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

function reduceGraph(current: GraphState, newData: Partial<GraphResponse>): GraphState {
  // Merge nodes (preserve existing positions and data, only add new nodes)
  const updatedNodes = newData.nodes
    ? [
        ...current.nodes, 
        ...newData.nodes.filter((newNode: Node) => 
          !current.nodes.some((existing: Node) => existing.id === newNode.id)
        )
      ]
    : current.nodes;

  const updatedEdges = newData.edges
    ? [
        ...current.edges.filter((existing: Edge) => 
          !newData.edges!.some((newEdge: Edge) => newEdge.id === existing.id)
        ),
        ...newData.edges
      ]
    : current.edges;

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

function addNode(node: Node) {
  const existingNode = state.nodes.find(n => n.id === node.id);
  if (existingNode) {
    const updatedNode = {
      ...node,
      position: existingNode.position 
    };
    state = { 
      ...state, 
      nodes: state.nodes.map(n => n.id === node.id ? updatedNode : n),
      lastUpdated: new Date().toISOString()
    };
  } else {
    state = { 
      ...state, 
      nodes: [...state.nodes, node],
      lastUpdated: new Date().toISOString()
    };
  }
}

function addTempNode(nodeType: string, position: { x: number, y: number }) {
  const tempId = `temp-${nodeType}-${Date.now()}`;
  
  const tempNode: Node = {
    id: tempId,
    type: nodeType,
    position,
    data: {
      label: `New ${nodeType}`,
      nodeType: nodeType,
      properties: {},
      state: NodeState.Loading,
      pins: [],
    }
  };
  
  addNode(tempNode);
  return tempId;
}

function updateNode(nodeId: string, updates: Partial<Node>) {
  state = {
    ...state,
    nodes: state.nodes.map(node => 
      node.id === nodeId ? { ...node, ...updates } : node
    ),
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

// Generic API call function using pin metadata 
async function fetchFromPin(nodeId: string, pinId: string, x: number = 0, y: number = 0) {
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
    const requestBody = {
      ...pin.metadata.parameters,
      x: Math.round(x),
      y: Math.round(y)
    };
    
    const response = await fetch(pin.metadata.apiEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody)
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    const graphResponse: GraphResponse = await response.json();
    
    // Create a new edge from the origin node to the new node
    if (graphResponse.nodes && graphResponse.nodes.length > 0) {
      const newNode = graphResponse.nodes[0];
      
      const newEdge: Edge = {
        id: `edge-${nodeId}-${newNode.id}`,
        source: nodeId,
        sourceHandle: pinId, 
        target: newNode.id,
        targetHandle: `${newNode.id}-input`, 
        type: 'default',
        data: {
          label: pin.label || 'Connection',
          edgeType: 'expansion',
          properties: {
            sourcePin: pinId,
            sourceNode: nodeId
          }
        }
      };
      
      const updatedGraphResponse: GraphResponse = {
        ...graphResponse,
        edges: [...(graphResponse.edges || []), newEdge]
      };
      
      updateGraph(updatedGraphResponse);
    } else {
      // No new nodes returned, just update the original response
      updateGraph(graphResponse);
    }



  } catch (error) {
    console.error('Failed to fetch from pin:', error);
    
    // Update node state to error
    const errorNode = {
      ...node,
      data: { ...node.data, state: NodeState.Error }
    };
    updateNode(nodeId, errorNode);
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


function getNode(nodeId: string): Node | undefined {
  return state.nodes.find(n => n.id === nodeId);
}

function getNodeEdges(nodeId: string): Edge[] {
  return state.edges.filter(e => e.source === nodeId || e.target === nodeId);
}

function getMetadata(queryId: string): GraphMetadata | undefined {
  return state.metadata.get(queryId);
}

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
  addNode, 
  addTempNode,
  removeNode,
  addEdge,
  removeEdge,
  clearGraph,
  getNode,
  getNodeEdges,
  getMetadata,
  updateNode
}; 