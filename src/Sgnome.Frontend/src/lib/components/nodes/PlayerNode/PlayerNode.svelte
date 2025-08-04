<script lang="ts">
    import type { NodeData } from "$lib/types/graph";
    import { PlayerPresenter } from "$lib/presenters/PlayerPresenter";
    import * as Card from "$lib/components/ui/card/index.js";
    import { Separator } from "$lib/components/ui/separator";
    import * as Accordion from "$lib/components/ui/accordion";
    import PlayerLibrariesWidget from "$lib/components/widgets/PlayerLibrariesWidget.svelte";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    import ContextToggle from "$lib/components/widgets/ContextToggle.svelte";
    import AllCard from "./AllCard.svelte";
    import SteamCard from "./SteamCard.svelte";
    import EpicCard from "./EpicCard.svelte";
    import XboxCard from "./XboxCard.svelte";
    import PlayStationCard from "./PlayStationCard.svelte";

    const { data, id } = $props<{ data: NodeData; id: string }>();

    let contexts = [
        { value: "all", label: "All" },
        { value: "steam", label: "Steam" },
        { value: "epic", label: "Epic" },
        { value: "xbox", label: "Xbox" },
        { value: "playstation", label: "PlayStation" },
    ];

    let selectedContext = $state("all");
    function handleContextChange(context: string) {
        selectedContext = context;
    }

    const presenter = new PlayerPresenter();

    // Extract player data from the node data properties
    const displayName = $derived(presenter.getDisplayName(data));
    const libraryCount = $derived(presenter.getLibraryCount(data));
    const avatarUrl = $derived(presenter.getAvatarUrl(data));
    const profilesBySource = $derived(presenter.getProfilesBySource(data));
    const availableProfileSources = $derived(
        presenter.getAvailableProfileSources(data),
    );
    const internalId = $derived(
        data.properties.InternalId as string | undefined,
    );
    const libraryPins = $derived(presenter.getLibraryPins(data));
</script>

<Card.Root class="w-80">
    <ContextToggle
        {contexts}
        bind:selectedContext
        onContextChange={handleContextChange}
    />
    <IOHandles hostId={id} />
    <Card.Header class="pb-2">
        <div class="flex items-center gap-3">
            {#if avatarUrl}
                <img
                    src={avatarUrl}
                    alt={displayName}
                    class="w-8 h-8 rounded-full object-cover"
                />
            {:else}
                <div
                    class="w-8 h-8 rounded-full bg-muted flex items-center justify-center"
                >
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
    <Separator />

    {#if selectedContext === "all"}
        <AllCard 
            playerNode={data}   
            {availableProfileSources}
            onContextChange={handleContextChange}
        />
    {:else if selectedContext === "steam"}
        <SteamCard 
            playerNode={data}
            {availableProfileSources}
            {profilesBySource}
        />
    {:else if selectedContext === "epic"}
        <EpicCard 
            {availableProfileSources}
            {profilesBySource}
        />
    {:else if selectedContext === "xbox"}
        <XboxCard 
            {availableProfileSources}
            {profilesBySource}
        />
    {:else if selectedContext === "playstation"}
        <PlayStationCard 
            {availableProfileSources}
            {profilesBySource}
        />
    {/if}

</Card.Root>



