<script lang="ts">
  import { SvelteFlow, Controls, Background } from '@xyflow/svelte';
  import { getState, selectNodeCount, selectEdgeCount, fetchFromPin, fetchWithEndpoint, clearGraph } from '../stores/graphState.svelte';
  import type { Node, Edge } from '../types/graph';
  import { NodeState, PinBehavior, PinState } from '../types/graph';
  import CustomNode from './CustomNode.svelte';

  // Test data for development
  const testSteamId = '76561197995791208';

  // Handle user interaction
  function handleFetchUserLibrary() {
    // Use the development helper to test with a canned endpoint
    fetchWithEndpoint('/api/player/select', {
      steamId: testSteamId,
      displayName: 'Test Player',
      epicId: null,
      identifiers: {}
    });
  }

  function handleClearGraph() {
    clearGraph();
  }

  // Create a test player node by calling the API
  async function handleAddTestPlayer() {
    // Use the development helper to test with a canned endpoint
    fetchWithEndpoint('/api/player/select', {
      steamId: testSteamId,
      displayName: 'Test Player',
      epicId: null,
      identifiers: {}
    });
  }

  // Handle pin expansion
  function handleExpandPin(nodeId: string, pinId: string) {
    fetchFromPin(nodeId, pinId);
  }

  // Register custom node types
  const nodeTypes = {
    default: CustomNode
  };

  // Handle custom node pin expansion
  function handleCustomNodeExpand(event: CustomEvent<{ nodeId: string; pinId: string }>) {
    fetchFromPin(event.detail.nodeId, event.detail.pinId);
  }

  // Set up global event listener for custom node pin expansion
  import { onMount } from 'svelte';
  
  onMount(() => {
    const handlePinExpand = (event: CustomEvent<{ nodeId: string; pinId: string }>) => {
      if (event.type === 'expandPin') {
        fetchFromPin(event.detail.nodeId, event.detail.pinId);
      }
    };

    document.addEventListener('expandPin', handlePinExpand as EventListener);
    
    return () => {
      document.removeEventListener('expandPin', handlePinExpand as EventListener);
    };
  });
</script>

<div class="graph-container">
  <!-- Minimal controls -->
  <div class="controls">
    <div class="stats">
      <span>Nodes: {selectNodeCount()}</span>
      <span>Edges: {selectEdgeCount()}</span>
    </div>
    <div class="actions">
      <button on:click={handleAddTestPlayer}>Add Test Player</button>
      <button on:click={handleFetchUserLibrary}>Fetch User Library</button>
      <button on:click={handleClearGraph}>Clear</button>
    </div>
  </div>

  <!-- Graph view -->
  <div class="graph-view">
    <SvelteFlow 
      nodes={getState().nodes} 
      edges={getState().edges}
      nodeTypes={nodeTypes}
    >
      <Background />
    </SvelteFlow>
  </div>
</div>

<style>
  .graph-container {
    display: flex;
    flex-direction: column;
    height: 100%;
    width: 100%;
  }

  .controls {
    padding: 8px 16px;
    background: #f5f5f5;
    border-bottom: 1px solid #ddd;
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-shrink: 0;
  }

  .stats {
    display: flex;
    gap: 16px;
    font-size: 14px;
    color: #666;
  }

  .actions {
    display: flex;
    gap: 8px;
  }

  .actions button {
    padding: 4px 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
    background: white;
    cursor: pointer;
    font-size: 12px;
  }

  .graph-view {
    flex: 1;
    position: relative;
    min-height: 0;
  }

  /* Make SvelteFlow fill the container */
  :global(.svelte-flow) {
    width: 100% !important;
    height: 100% !important;
  }

  /* Ensure the viewport takes full space */
  :global(.svelte-flow__viewport) {
    width: 100% !important;
    height: 100% !important;
  }

  /* Ensure the canvas takes full space */
  :global(.svelte-flow__canvas) {
    width: 100% !important;
    height: 100% !important;
  }

  /* Ensure the background fills the viewport */
  :global(.svelte-flow__background) {
    width: 100% !important;
    height: 100% !important;
  }

  /* Make nodes visible with basic styling */
  :global(.svelte-flow__node) {
    background: white;
    border: 1px solid #ccc;
    border-radius: 4px;
    padding: 4px 8px;
    min-width: 80px;
    text-align: center;
    font-size: 12px;
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  }

  /* Make edges visible */
  :global(.svelte-flow__edge-path) {
    stroke: #007bff;
    stroke-width: 2;
  }
</style> 