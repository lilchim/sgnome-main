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
    {#if !availableProfileSources.includes("epic")}
        <div class="text-center py-4">
            <p class="text-sm text-muted-foreground mb-2">
                No Epic account linked
            </p>
            <p class="text-xs text-muted-foreground">
                Add Epic ID to see Epic profile data
            </p>
            <!-- TODO: Add form to link Epic account -->
        </div>
    {:else}
        <div class="space-y-3">
            <h4 class="text-sm font-medium">Epic Profile</h4>
            {#each profilesBySource["epic"] as pin}
                <div class="text-sm">
                    <span class="font-medium">{pin.label}:</span> {pin.summary.displayText}
                </div>
            {/each}
        </div>
    {/if}
</Card.Content> 