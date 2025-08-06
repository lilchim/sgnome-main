<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import { Separator } from "$lib/components/ui/separator";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import PlayerRecentActivityWidget from "$lib/components/widgets/PlayerRecentActivity.svelte";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import { SteamIcon, EpicIcon, XboxIcon, PlayStationIcon } from "$lib/components/ui/icons";
    import { Button } from "$lib/components/ui/button";

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
    let recentActivityPins = $derived(playerPresenter.getRecentActivityPins(playerNode));
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
                    <Button variant="outline"
                        size="sm"
                        class="flex items-center gap-2"
                        onclick={() => onContextChange("steam")}
                    >
                        <SteamIcon size={16} class="text-blue-600" />
                        Add Steam
                    </Button>
                    <Button variant="outline"
                        size="sm"
                        class="flex items-center gap-2"
                        onclick={() => onContextChange("epic")}
                    >
                        <EpicIcon size={16} class="text-purple-600" />
                        Add Epic
                    </Button>
                    <Button variant="outline"
                        size="sm"
                        class="flex items-center gap-2"
                        onclick={() => onContextChange("xbox")}
                    >   
                        <XboxIcon size={16} class="text-green-600" />
                        Add Xbox
                    </Button>
                    <Button
                        variant="outline"   
                        size="sm"
                        class="flex items-center gap-2"
                        onclick={() => onContextChange("playstation")}
                    >
                        <PlayStationIcon size={16} class="text-blue-500" />
                        Add PlayStation
                    </Button>
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
                        <Button
                            variant="outline"
                            size="sm"
                            class="flex items-center gap-2"
                            onclick={() => onContextChange("steam")}
                        >
                            <SteamIcon size={16} class="text-blue-600" />
                            Add Steam
                        </Button>
                    {/if}
                    {#if !availableProfileSources.includes("epic")}
                        <Button
                            variant="outline"
                            size="sm"
                            class="flex items-center gap-2"
                            onclick={() => onContextChange("epic")}
                        >
                            <EpicIcon size={16} class="text-purple-600" />
                            Add Epic
                        </Button>
                    {/if}
                    {#if !availableProfileSources.includes("xbox")}
                        <Button
                            variant="outline"
                            size="sm"
                            class="flex items-center gap-2"
                            onclick={() => onContextChange("xbox")}
                        >
                            <XboxIcon size={16} class="text-green-600" />
                            Add Xbox
                        </Button>
                    {/if}
                    {#if !availableProfileSources.includes("playstation")}
                        <Button
                            variant="outline"
                            size="sm"
                            class="flex items-center gap-2"
                            onclick={() => onContextChange("playstation")}
                        >
                            <PlayStationIcon size={16} class="text-blue-500" />
                            Add PlayStation
                        </Button>
                    {/if}
                </div>
            </div>
        </div>

        <Separator />
        <PlayerRecentActivityWidget pins={recentActivityPins} />
        <Separator />
        <PlayerLibrariesWidget {libraryPins} />
    </div>
    {/if}
</Card.Content>
