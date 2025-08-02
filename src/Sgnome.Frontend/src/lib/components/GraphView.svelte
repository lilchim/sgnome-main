<script lang="ts">
  import { SvelteFlow, Background } from "@xyflow/svelte";
  import { getState, fetchFromPin } from "../stores/graphState.svelte";
  import PlayerNode from "./nodes/PlayerNode.svelte";
  import LibraryNode from "./nodes/LibraryNode.svelte";
  import GraphHeader from "./GraphHeader.svelte";
  import type { OnConnectStartParams } from "@xyflow/svelte";
  import GameNode from "./nodes/GameNode.svelte";
  import { convertScreenToCanvas } from "$lib/util/convertScreenToCanvas";

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
    // Only process if we were actually connecting
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
</script>

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
      minZoom={0.2}
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
