<script lang="ts">
    import type { Pin } from "$lib/types/graph";
    import { Handle, Position } from "@xyflow/svelte";

    const { gamePins } = $props<{ gamePins: Pin[] }>();

    const selectedGames = $state<Pin[]>([]);
</script>

<div>
    {#each gamePins as gamePin}
        <div class="flex justify-between">
            
            <h1>{gamePin.label}</h1>
            <Handle
                id={gamePin.id}
                type="source"
                position={Position.Right}
                class="pin-handle"
            />
        </div>
    {/each}
</div>
