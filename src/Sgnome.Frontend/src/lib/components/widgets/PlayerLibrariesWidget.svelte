<script lang="ts">
    import { Handle, Position } from "@xyflow/svelte";
    import type { Pin } from "$lib/types/graph";
    export let libraryPins: Pin[];
</script>

<div>
    {#if libraryPins.length === 0}
        <span class="text-lg font-semibold">No Libraries</span>
    {:else if libraryPins.length === 1}
        <span class="text-lg font-semibold">Library</span>
    {:else}
        <span class="text-lg font-semibold">{libraryPins.length} Libraries</span
        >
    {/if}
    {#each libraryPins as pin}
        <div class="flex items-center justify-between">
            <span>{pin.summary.source}</span>
            <span>{pin.summary.displayText}</span>
            <Handle
                type="source"
                position={Position.Right}
                id={pin.id}
                class="pin-handle"
            />
        </div>
    {/each}
</div>
