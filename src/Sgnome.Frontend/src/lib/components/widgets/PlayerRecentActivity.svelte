<script lang="ts">
    import type { Pin } from "$lib/types/graph";
    import { Handle, Position } from "@xyflow/svelte";
    import { KeyValueDisplay } from "../ui/key-value-display";


    let { pins }: { pins: Pin[] } = $props();

    const formatPlaytime = (playtime: number) => {
        const hours = Math.floor(playtime / 3600);
        const minutes = Math.floor((playtime % 3600) / 60);
        return `${hours}h ${minutes}m`;
    }

</script>

<div class="text-lg font-semibold">Recent Activity</div>
{#each pins as pin}
    <div class="flex items-center justify-between">
        <KeyValueDisplay label={pin.summary.displayText} value={formatPlaytime(pin.summary.preview['playtime-forever'] as number)} />
        <Handle
            type="source"
            position={Position.Right}
            id={pin.id}
            class="pin-handle"
        />
    </div>
{/each}