<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import PlayerRecentActivityWidget from "$lib/components/widgets/PlayerRecentActivity.svelte";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import { Input } from "$lib/components/ui/input";
    import { Button } from "$lib/components/ui/button";
    import { Badge } from "$lib/components/ui/badge";
    import { KeyValueDisplay } from "$lib/components/ui/key-value-display";
    import { SteamIcon } from "$lib/components/ui/icons";
    import { addSteamIdToPlayer } from "$lib/stores/graphState.svelte";
    import type { Pin } from "$lib/types/graph";
    import { Separator } from "$lib/components/ui/separator";
    import type { NodeData } from "$lib/types/graph";

    let {
        playerNode = $bindable({} as NodeData),
        nodeId = $bindable(""),
        availableProfileSources = $bindable([]),
        profilesBySource = $bindable({} as Record<string, Pin[]>),
    } = $props<{
        playerNode: NodeData;
        nodeId: string;
        availableProfileSources: string[];
        profilesBySource: Record<string, Pin[]>;
    }>();

    const playerPresenter = new PlayerPresenter();
    let libraryPins = $derived(playerPresenter.getLibraryPins(playerNode));
    let steamPlayerProfile = $derived(
        playerPresenter.getSteamPlayerProfile(playerNode),
    );
    let recentActivityPins = $derived(playerPresenter.getRecentActivityPins(playerNode).filter(pin => pin.summary.source === "steam"));

    let steamId = $state("");
    let isSubmitting = $state(false);

    async function handleSubmit() {
        if (!steamId.trim()) return;

        isSubmitting = true;
        try {
            await addSteamIdToPlayer(nodeId, steamId.trim());
        } catch (error) {
            console.error("Failed to add Steam ID:", error);
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
                    <label for="steam-id" class="text-sm font-medium"
                        >Steam ID</label
                    >
                    <Input
                        id="steam-id"
                        bind:value={steamId}
                        placeholder="Enter Steam ID (e.g., 76561234012341238)"
                        onkeydown={(event) => {
                            if (event.key === "Enter") {
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
                    {isSubmitting ? "Adding..." : "Add Steam ID"}
                </Button>
            </div>
        </div>
    {:else}
        <div class="space-y-3">
            <div class="space-y-1">
                <div class="flex-col mb-4">
                    <div class="flex items-center gap-2">
                        <SteamIcon size={20} />
                        <h3 class="text-lg font-semibold">Steam Profile</h3>
                    </div>
                    {#if steamPlayerProfile.status}
                        <Badge
                            variant={steamPlayerProfile.status === "online"
                                ? "default"
                                : "secondary"}
                        >
                            {steamPlayerProfile.status}
                        </Badge>
                    {/if}
                </div>

                <KeyValueDisplay label="Profile URL">
                    <Button
                        variant="link"
                        href={steamPlayerProfile.profileUrl}
                        class="justify-start p-0"
                    >
                        <span class="truncate"
                            >{steamPlayerProfile.profileUrl}</span
                        >
                    </Button>
                </KeyValueDisplay>

                <KeyValueDisplay
                    label="Created At"
                    value={steamPlayerProfile.createdAt}
                />
            </div>
            <Separator />
            <PlayerRecentActivityWidget pins={recentActivityPins} />
            <Separator />
            <PlayerLibrariesWidget {libraryPins} />
        </div>
    {/if}
</Card.Content>
