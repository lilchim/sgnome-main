<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import { addSteamIdToPlayer } from "$lib/stores/graphState.svelte";
    import type { Pin } from "$lib/types/graph";

    let {
        playerNode = $bindable({} as NodeData),
        nodeId = $bindable(""),
        availableProfileSources = $bindable([]),
        profilesBySource = $bindable({} as Record<string, Pin[]>)
    } = $props<{
        playerNode: NodeData;
        nodeId: string;
        availableProfileSources: string[];
        profilesBySource: Record<string, Pin[]>;
    }>();

    const playerPresenter = new PlayerPresenter();
    let libraryPins = $derived(playerPresenter.getLibraryPins(playerNode));
    
    let steamId = $state("");
    let isSubmitting = $state(false);

    async function handleSubmit() {
        if (!steamId.trim()) return;
        
        isSubmitting = true;
        try {
            await addSteamIdToPlayer(nodeId, steamId.trim());
        } catch (error) {
            console.error('Failed to add Steam ID:', error);
        } finally {
            isSubmitting = false;
        }
    }
</script>

<Card.Content class="pt-4">
    {#if !availableProfileSources.includes("steam")}
        <div class="space-y-4">
            <div class="text-center">
                <p class="text-sm text-muted-foreground mb-2">
                    No Steam account linked
                </p>
                <p class="text-xs text-muted-foreground">
                    Add Steam ID to see Steam profile data
                </p>
            </div>
            
            <div class="space-y-3">
                <div class="space-y-2">
                    <label for="steam-id" class="text-sm font-medium">Steam ID</label>
                    <Input 
                        id="steam-id"
                        bind:value={steamId}
                        placeholder="Enter Steam ID (e.g., 76561234012341238)"
                        onkeydown={(event) => {
                            if (event.key === 'Enter') {
                                handleSubmit();
                            }
                        }}
                    />
                </div>
                
                <Button 
                    onclick={handleSubmit}
                    disabled={!steamId.trim() || isSubmitting}
                    class="w-full"
                >
                    {isSubmitting ? 'Adding...' : 'Add Steam ID'}
                </Button>
            </div>
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