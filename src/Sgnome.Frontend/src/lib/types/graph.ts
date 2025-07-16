// Graph data types that match the C# backend models and xyflow expectations
export interface Node {
  id: string;
  type: string;
  position: { x: number; y: number }; // xyflow expects position object
  data: NodeData;
}

export interface NodeData {
  label: string;
  nodeType: string;
  properties: Record<string, unknown>;
  pins: Pin[];
  state: NodeState;
  [key: string]: unknown; // Add index signature for xyflow compatibility
}

export enum NodeState {
  Loading = 'loading',
  Loaded = 'loaded',
  Error = 'error'
}

export interface Pin {
  id: string;
  label: string;
  type: string; // "recently-played", "publisher", "release-date", etc.
  state: PinState;
  behavior: PinBehavior;
  summary: PinSummary;
  metadata?: PinMetadata;
}

export interface PinSummary {
  displayText: string;
  count?: number;
  icon?: string;
  preview: Record<string, unknown>;
}

export interface PinMetadata {
  targetNodeType: string;
  targetNodeId?: string;
  originNodeId?: string;
  apiEndpoint: string;
  parameters: Record<string, unknown>;
}

export enum PinState {
  Unexpanded = 'unexpanded',
  Loading = 'loading',
  Expanded = 'expanded'
}

export enum PinBehavior {
  Expandable = 'expandable',
  Informational = 'informational'
}

export interface Edge {
  id: string;
  source: string;
  target: string;
  type: string;
  data: EdgeData;
}

export interface EdgeData {
  label: string;
  edgeType: string;
  properties: Record<string, unknown>;
  [key: string]: unknown; // Add index signature for xyflow compatibility
}

// Backend GraphMetadata structure
export interface GraphMetadata {
  queryType: string;
  queryId: string;
  timestamp: string;
  context: Record<string, unknown>;
}

export interface GraphResponse {
  nodes: Node[];
  edges: Edge[];
  metadata: GraphMetadata;
}

// Graph state interface for the state module
export interface GraphState {
  nodes: Node[];
  edges: Edge[];
  metadata: Map<string, GraphMetadata>;
  lastUpdated: string;
} 