<script lang="ts">
  import { SvelteFlow, Controls, Background } from '@xyflow/svelte';
  import { getState, fetchFromPin } from '../stores/graphState.svelte';
  import type { Node, Edge } from '../types/graph';
  import { NodeState, PinBehavior, PinState } from '../types/graph';
  import CustomNode from './CustomNode.svelte';
  import PlayerNode from './nodes/playerNode/PlayerNode.svelte';
  import GraphHeader from './GraphHeader.svelte';

  // Register custom node types
  const nodeTypes = {
    // default: CustomNode,
    player: PlayerNode
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
  <GraphHeader />
  
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

  .graph-view {
    flex: 1;
    position: relative;
    min-height: 0;
  }
</style> 