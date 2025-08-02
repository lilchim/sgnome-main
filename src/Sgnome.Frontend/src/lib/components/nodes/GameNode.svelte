<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import type { NodeData, Pin } from "$lib/types/graph";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    import { GamePresenter } from "$lib/presenters/GamePresenter";
    import * as Tabs from "$lib/components/ui/tabs";
    import * as ToggleGroup from "$lib/components/ui/toggle-group";

    const { data, id } = $props<{ data: NodeData; id: string }>();

    const gamePresenter = new GamePresenter();
    const steamGameData = $derived(gamePresenter.getSteamGameData(data));

    let selectedTab = $state("steam");
    const setSelectedTab = (value: string) => {
        selectedTab = value;
    };
</script>

<Card.Root style="width: 480px;">
    <div style="position: absolute; top: -18px; right: 16px;">
        <ToggleGroup.Root
            type="single"
            value={selectedTab}
            onValueChange={setSelectedTab}
        >
            <ToggleGroup.Item value="steam">Steam</ToggleGroup.Item>
            <ToggleGroup.Item value="rawg">RAWG</ToggleGroup.Item>
        </ToggleGroup.Root>
    </div>
    <IOHandles hostId={id}></IOHandles>

    {#if selectedTab === "steam"}
        {@render steamCard()}
    {:else if selectedTab === "rawg"}
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
