<script lang="ts">
  import { SvelteFlow, Background } from "@xyflow/svelte";
  import { getState, fetchFromPin, fetchWithEndpoint } from "../stores/graphState.svelte";
  import PlayerNode from "./nodes/PlayerNode.svelte";
  import LibraryNode from "./nodes/LibraryNode.svelte";
  import GraphHeader from "./GraphHeader.svelte";
  import type { OnConnectStartParams } from "@xyflow/svelte";
  import GameNode from "./nodes/GameNode.svelte";
  import { convertScreenToCanvas } from "$lib/util/convertScreenToCanvas";
  import CanvasContextMenu from "./CanvasContextMenu.svelte";
  import { CANVAS_CONTEXT_MENU_EVENTS } from "$lib/constants/canvasContextMenuEvents";
  // Register custom node types
  const nodeTypes = {
    // default: CustomNode,
    player: PlayerNode,
    library: LibraryNode,
    GameNode: GameNode,
  };

  // Events to handle pin expansion
  let isConnecting = false; // Track if we're in a connection
  let connectionStartData: OnConnectStartParams | null = null; // Stores connection start info

  // Context menu state
  let contextMenuOpen = $state(false);
  let contextMenuPosition = $state({ x: 0, y: 0 });
  let canvasPosition = $state({ x: 0, y: 0 });

  const handleConnectStart = (
    event: MouseEvent | TouchEvent,
    connectionState: OnConnectStartParams | null,
  ) => {
    console.log("Connection started:", connectionState);
    isConnecting = true;
    connectionStartData = connectionState;
  };

  const handleConnectEnd = (
    event: MouseEvent | TouchEvent,
    connectionState: any,
  ) => {
    if (!isConnecting) {
      return;
    }

    console.log("Connection ended:", connectionState);

    // Check if we dropped in empty space (no target)
    if (!connectionState?.toNode) {
      const { clientX, clientY } =
        "changedTouches" in event ? event.changedTouches[0] : event;

      console.log("Dropped in empty space at:", { clientX, clientY });

      // Convert screen coordinates to canvas coordinates
      const { x: canvasX, y: canvasY } = convertScreenToCanvas(
        clientX,
        clientY,
      );

      // Get the pin data from the connection start
      if (connectionStartData?.handleId && connectionStartData?.nodeId) {
        const pinId = connectionStartData.handleId;
        const nodeId = connectionStartData.nodeId;
        console.log("fetching pin", nodeId, pinId);

        fetchFromPin(nodeId, pinId, canvasX, canvasY);
      }
    }

    // Reset connection state
    isConnecting = false;
    connectionStartData = null;
  };

  // Handler for xyflow "oncontextmenu" which fires when right clicking the canvas
  function handleCanvasContextMenu(event: MouseEvent) {
    event.preventDefault();
    
    const target = event.target as HTMLElement;
    const isNode = target.closest('[data-id]') || target.closest('.svelte-flow__node');
    
    if (isNode) {
      return; 
    }

    openContextMenu(event);
  }

  function openContextMenu(event: MouseEvent) {
    event.preventDefault();
    
    const { clientX, clientY } = event;
    const { x: canvasX, y: canvasY } = convertScreenToCanvas(clientX, clientY);
    console.log(`open context menu at ${clientX}, ${clientY}`);
    
    // Set context menu position and canvas position
    contextMenuPosition = { x: clientX, y: clientY };
    canvasPosition = { x: canvasX, y: canvasY };
    contextMenuOpen = true;
  }

  // Handle context menu actions
  function handleContextMenuAction(action: string) {
    console.log(`Action: ${action} at canvas position:`, $state.snapshot(canvasPosition));
    const x = Math.round(canvasPosition.x);
    const y = Math.round(canvasPosition.y);
    
    switch (action) {
      case CANVAS_CONTEXT_MENU_EVENTS.ADD_NODE_PLAYER:
        // TODO: Implement add node at position
        console.log('Add player at:', $state.snapshot(canvasPosition));
        fetchWithEndpoint("/api/player/select", {
            identifiers: {
                steam: "76561197995791208",
            },
            x,
            y
        });
        break;
      case CANVAS_CONTEXT_MENU_EVENTS.CENTER_VIEW:
        // TODO: Implement center view on position
        console.log('Center view on:', $state.snapshot(canvasPosition));
        break;
      case CANVAS_CONTEXT_MENU_EVENTS.FIT_VIEW:
        // TODO: Implement fit to view
        console.log('Fit to view');
        break;
      case CANVAS_CONTEXT_MENU_EVENTS.CLEAR_GRAPH:
        // TODO: Implement clear graph
        console.log('Clear graph');
        break;
    }
    
    contextMenuOpen = false;
  }

  // Close context menu when clicking outside
  function handleGlobalClick(event: MouseEvent) {
    if (contextMenuOpen) {
      contextMenuOpen = false;
    }
  }
</script>

<svelte:window on:click={handleGlobalClick} />

<div class="graph-container">
  <GraphHeader />

  <!-- Graph view -->
  <div class="graph-view">
    <SvelteFlow
      nodes={getState().nodes}
      edges={getState().edges}
      {nodeTypes}
      onconnectstart={(event, connectionState) =>
        handleConnectStart(event, connectionState)}
      onconnectend={(event, connectionState) =>
        handleConnectEnd(event, connectionState)}
      oncontextmenu={handleCanvasContextMenu}
      minZoom={0.2}
    >
      <Background />
    </SvelteFlow>
  </div>
</div>

<!-- Canvas Context Menu -->
<CanvasContextMenu 
  bind:open={contextMenuOpen}
  bind:position={contextMenuPosition}
  onAction={handleContextMenuAction}
/>

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
