<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import { Separator } from "$lib/components/ui/separator";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import ExampleUsage from "$lib/components/ui/key-value-display/example-usage.svelte";

    let {
        playerNode = $bindable({} as Node),
        availableProfileSources = $bindable([]),
        onContextChange = $bindable((context: string) => {}),
    } = $props<{
        playerNode: Node;
        availableProfileSources: string[];
        onContextChange: (context: string) => void;
    }>();

    const playerPresenter = new PlayerPresenter();
    let libraryPins = $derived(playerPresenter.getLibraryPins(playerNode));
</script>

<Card.Content>
    {#if availableProfileSources.length === 0}
        <div class="text-center py-4">
            <p class="text-sm text-muted-foreground mb-2">
                No platform accounts linked yet
            </p>
            <p class="text-xs text-muted-foreground">
                Link a platform account to see player data
            </p>
            <div class="mt-4 space-y-2">
                <p class="text-xs text-muted-foreground">
                    Add platform accounts:
                </p>
                <div class="flex flex-wrap gap-2 justify-center">
                    <button
                        class="px-3 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                        onclick={() => onContextChange("steam")}
                    >
                        Add Steam
                    </button>
                    <button
                        class="px-3 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                        onclick={() => onContextChange("epic")}
                    >
                        Add Epic
                    </button>
                    <button
                        class="px-3 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                        onclick={() => onContextChange("xbox")}
                    >
                        Add Xbox
                    </button>
                    <button
                        class="px-3 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                        onclick={() => onContextChange("playstation")}
                    >
                        Add PlayStation
                    </button>
                </div>
            </div>
        </div>
    {:else}
    <div class="space-y-3">
        <div class="space-y-3">
            <h4 class="text-lg font-semibold">Linked Platforms</h4>
            <div class="flex flex-wrap gap-2">
                {#each availableProfileSources as source}
                    <button
                        class="px-2 py-1 text-xs bg-primary text-primary-foreground rounded hover:bg-primary/90"
                        onclick={() => onContextChange(source)}
                    >
                        {source}
                    </button>
                {/each}
            </div>
            <div class="pt-2 border-t">
                <p class="text-xs text-muted-foreground mb-2">
                    Add more platforms:
                </p>
                <div class="flex flex-wrap gap-2">
                    {#if !availableProfileSources.includes("steam")}
                        <button
                            class="px-2 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                            onclick={() => onContextChange("steam")}
                        >
                            Add Steam
                        </button>
                    {/if}
                    {#if !availableProfileSources.includes("epic")}
                        <button
                            class="px-2 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                            onclick={() => onContextChange("epic")}
                        >
                            Add Epic
                        </button>
                    {/if}
                    {#if !availableProfileSources.includes("xbox")}
                        <button
                            class="px-2 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                            onclick={() => onContextChange("xbox")}
                        >
                            Add Xbox
                        </button>
                    {/if}
                    {#if !availableProfileSources.includes("playstation")}
                        <button
                            class="px-2 py-1 text-xs bg-secondary text-secondary-foreground rounded hover:bg-secondary/80"
                            onclick={() => onContextChange("playstation")}
                        >
                            Add PlayStation
                        </button>
                    {/if}
                </div>
            </div>
        </div>

        <Separator />
        <PlayerLibrariesWidget {libraryPins} />
    </div>
    {/if}
</Card.Content>
