<script lang="ts">
  import { getState, selectNodeCount, selectEdgeCount, addNode, clearGraph } from '../stores/graphState.svelte';
  import type { Node } from '../types/graph';
  import { NodeState } from '../types/graph';

  let testNodeId = 1;

  function handleAddTestNode() {
    const testNode: Node = {
      id: `test-node-${testNodeId}`,
      type: 'default',
      position: { 
        x: Math.random() * 600 + 100,
        y: Math.random() * 400 + 100
      },
      data: {
        label: `Test Node ${testNodeId}`,
        nodeType: 'test',
        properties: { testId: testNodeId },
        pins: [],
        state: NodeState.Loaded
      }
    };

    addNode(testNode);
    testNodeId++;
  }

  function handleClearGraph() {
    clearGraph();
    testNodeId = 1;
  }
</script>

<div class="state-test">
  <h3>State Management Test</h3>
  
  <div class="stats">
    <p>Current Nodes: {selectNodeCount()}</p>
    <p>Current Edges: {selectEdgeCount()}</p>
    <p>Last Updated: {getState().lastUpdated || 'Never'}</p>
  </div>

  <div class="actions">
    <button on:click={handleAddTestNode}>Add Test Node</button>
    <button on:click={handleClearGraph}>Clear Graph</button>
  </div>

  <div class="node-list">
    <h4>Current Nodes:</h4>
    {#if getState().nodes.length === 0}
      <p>No nodes in graph</p>
    {:else}
      <ul>
        {#each getState().nodes as node}
          <li>{node.data.label} (ID: {node.id})</li>
        {/each}
      </ul>
    {/if}
  </div>
</div>

<style>
</style> 