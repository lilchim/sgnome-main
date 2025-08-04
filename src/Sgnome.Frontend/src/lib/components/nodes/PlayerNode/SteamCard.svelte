<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import type { Pin } from "$lib/types/graph";

    let {
        playerNode = $bindable({} as Node),
        availableProfileSources = $bindable([]),
        profilesBySource = $bindable({} as Record<string, Pin[]>)
    } = $props<{
        playerNode: Node;
        availableProfileSources: string[];
        profilesBySource: Record<string, Pin[]>;
    }>();

    const playerPresenter = new PlayerPresenter();
    let libraryPins = $derived(playerPresenter.getLibraryPins(playerNode));
</script>

<Card.Content class="pt-4">
    {#if !availableProfileSources.includes("steam")}
        <div class="text-center py-4">
            <p class="text-sm text-muted-foreground mb-2">
                No Steam account linked
            </p>
            <p class="text-xs text-muted-foreground">
                Add Steam ID to see Steam profile data
            </p>
            <!-- TODO: Add form to link Steam account -->
        </div>
    {:else}
        <div class="space-y-3">
            <h4 class="text-sm font-medium">Steam Profile</h4>
            {#each profilesBySource["steam"] as pin}
                <div class="text-sm">
                    <span class="font-medium">{pin.label}:</span> {pin.summary.displayText}
                </div>
            {/each}
        </div>
        <PlayerLibrariesWidget {libraryPins} />
    {/if}
</Card.Content> 