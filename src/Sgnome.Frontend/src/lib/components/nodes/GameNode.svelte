<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import type { NodeData, Pin } from "$lib/types/graph";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    import ContextToggle from "$lib/components/widgets/ContextToggle.svelte";
    import { GamePresenter } from "$lib/presenters/GamePresenter";

    const { data, id } = $props<{ data: NodeData; id: string }>();

    const gamePresenter = new GamePresenter();
    const steamGameData = $derived(gamePresenter.getSteamGameData(data));

    let selectedContext = $state("steam");
    
    const platforms = [
        { value: "steam", label: "Steam" },
        { value: "rawg", label: "RAWG" }
    ];

    function handleContextChange(context: string) {
        selectedContext = context;
    }
</script>

<Card.Root style="width: 480px;">
    <ContextToggle 
        contexts={platforms}
        bind:selectedContext
        onContextChange={handleContextChange}
    />
    <IOHandles hostId={id}></IOHandles>

    {#if selectedContext === "steam"}
        {@render steamCard()}
    {:else if selectedContext === "rawg"}
        {@render rawgCard()}
    {/if}
</Card.Root>

{#snippet steamCard()}
    <img
        src={steamGameData?.headerImageUrl}
        alt={steamGameData?.gameName}
        style="width: 100%; height: 100%; object-fit: cover;"
    />
    <Card.Content>
        <div class="flex flex-col gap-2">
            <span>Name: {steamGameData?.gameName}</span>
            <span>App ID: {steamGameData?.appId}</span>
            <span>Release Date</span>
            <span>Publisher</span>
            <span>Developer</span>
            <span>Genre</span>
            <span>Platforms</span>
            <span>Tags</span>
            <span>Description</span>
            <span>Website</span>
        </div>
    </Card.Content>
{/snippet}

{#snippet rawgCard()}
    Under Construction!
{/snippet}
