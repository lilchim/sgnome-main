<script lang="ts">
    import { Handle, Position } from "@xyflow/svelte";
    import type { Pin } from "$lib/types/graph";
    import {
        SteamIcon,
        EpicIcon,
        XboxIcon,
        PlayStationIcon,
    } from "$lib/components/ui/icons";

    let { libraryPins } = $props<{ libraryPins: Pin[] }>();
</script>

{#snippet steamIcon()}
    <SteamIcon size={16} class="text-blue-600" />
{/snippet}
{#snippet epicIcon()}
    <EpicIcon size={16} class="text-purple-600" />
{/snippet}
{#snippet xboxIcon()}
    <XboxIcon size={16} class="text-green-600" />
{/snippet}
{#snippet playstationIcon()}
    <PlayStationIcon size={16} class="text-blue-500" />
{/snippet}

<div class="space-y-2">
    {#if libraryPins.length === 0}
        <div class="text-lg font-semibold">No Libraries</div>
    {:else if libraryPins.length === 1}
        <div class="text-lg font-semibold">Library</div>
    {:else}
        <div class="text-lg font-semibold">
            {libraryPins.length} Libraries
        </div>
    {/if}
    {#each libraryPins as pin}
        <div class="flex items-center justify-between">
            {#if pin.summary.source === "steam"}
                {@render steamIcon()}
            {:else if pin.summary.source === "epic"}
                {@render epicIcon()}
            {:else if pin.summary.source === "xbox"}
                {@render xboxIcon()}
            {:else if pin.summary.source === "playstation"}
                {@render playstationIcon()}
            {/if}
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
