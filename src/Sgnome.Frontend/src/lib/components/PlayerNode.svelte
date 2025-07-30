<script lang="ts">
    import type { NodeData } from "../types/graph";
    import { PinBehavior, PinState } from "../types/graph";
    import * as Card from "$lib/components/ui/card/index.js";

    export let data: NodeData;
    export let id: string; // The node ID from SvelteFlow
    
    // Extract player data from the node data properties
    $: playerData = (data?.properties as any) || {};
    $: displayName = (playerData.displayName as string) || "Unknown Player";
    $: avatarUrl = playerData.avatarUrl as string | undefined;
    $: internalId = playerData.internalId as string | undefined;
    $: identifiers = playerData.identifiers as Record<string, string> | undefined;
</script>

<Card.Root class="w-80">
    <Card.Header class="pb-2">
        <div class="flex items-center gap-3">
            {#if avatarUrl}
                <img 
                    src={avatarUrl} 
                    alt={displayName}
                    class="w-8 h-8 rounded-full object-cover"
                />
            {:else}
                <div class="w-8 h-8 rounded-full bg-muted flex items-center justify-center">
                    <span class="text-xs font-medium text-muted-foreground">
                        {displayName.charAt(0).toUpperCase()}
                    </span>
                </div>
            {/if}
            <div class="flex-1 min-w-0">
                <Card.Title class="text-sm font-medium truncate">
                    {displayName}
                </Card.Title>
                {#if internalId}
                    <Card.Description class="text-xs text-muted-foreground">
                        ID: {internalId}
                    </Card.Description>
                {/if}
            </div>
        </div>
    </Card.Header>
    
    <Card.Content class="pt-0">
        {#if identifiers && Object.keys(identifiers).length > 0}
            <div class="space-y-1">
                <p class="text-xs font-medium text-muted-foreground">Identifiers:</p>
                <div class="space-y-1">
                    {#each Object.entries(identifiers) as [key, value]}
                        <div class="flex justify-between text-xs">
                            <span class="text-muted-foreground">{key}:</span>
                            <span class="font-mono text-xs truncate max-w-[200px]" title={value}>
                                {value}
                            </span>
                        </div>
                    {/each}
                </div>
            </div>
        {:else}
            <p class="text-xs text-muted-foreground">No additional identifiers</p>
        {/if}
    </Card.Content>
</Card.Root>
