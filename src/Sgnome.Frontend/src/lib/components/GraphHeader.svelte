<script lang="ts">
    import {
        selectNodeCount,
        selectEdgeCount,
        fetchWithEndpoint,
        clearGraph,
    } from "../stores/graphState.svelte";

    // Test data for development
    const testSteamId = "76561197995791208";

    function handleClearGraph() {
        clearGraph();
    }

    // Create a test player node by calling the API
    async function handleAddTestPlayer() {
        fetchWithEndpoint("/api/player/select", {
            identifiers: {
                steam: "76561197995791208",
            },
        });
    }
</script>

<div class="graph-header">
    <div class="stats">
        <span>Nodes: {selectNodeCount()}</span>
        <span>Edges: {selectEdgeCount()}</span>
    </div>
    <div class="actions">
        <button on:click={handleAddTestPlayer}>Add Test Player</button>
        <button on:click={handleClearGraph}>Clear</button>
    </div>
</div>

<style>
    .graph-header {
        padding: 8px 16px;
        background: var(--background);
        border-bottom: 1px solid var(--border);
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-shrink: 0;
    }

    .stats {
        display: flex;
        gap: 16px;
        font-size: 14px;
        color: var(--muted-foreground);
    }

    .actions {
        display: flex;
        gap: 8px;
    }

    .actions button {
        padding: 4px 8px;
        border: 1px solid var(--border);
        border-radius: var(--radius);
        background: var(--secondary);
        color: var(--secondary-foreground);
        cursor: pointer;
        font-size: 12px;
        transition: all 0.2s;
    }

    .actions button:hover {
        background: var(--accent);
        color: var(--accent-foreground);
    }
</style>
