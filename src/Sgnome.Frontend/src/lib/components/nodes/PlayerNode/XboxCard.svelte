<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import type { Pin } from "$lib/types/graph";

    let {
        availableProfileSources = $bindable([]),
        profilesBySource = $bindable({} as Record<string, Pin[]>)
    } = $props<{
        availableProfileSources: string[];
        profilesBySource: Record<string, Pin[]>;
    }>();
</script>

<Card.Content class="pt-4">
    {#if !availableProfileSources.includes("xbox")}
        <div class="text-center py-4">
            <p class="text-sm text-muted-foreground mb-2">
                No Xbox account linked
            </p>
            <p class="text-xs text-muted-foreground">
                Add Xbox ID to see Xbox profile data
            </p>
            <!-- TODO: Add form to link Xbox account -->
        </div>
    {:else}
        <div class="space-y-3">
            <h4 class="text-sm font-medium">Xbox Profile</h4>
            {#each profilesBySource["xbox"] as pin}
                <div class="text-sm">
                    <span class="font-medium">{pin.label}:</span> {pin.summary.displayText}
                </div>
            {/each}
        </div>
    {/if}
</Card.Content> 