<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import type { NodeData, Pin } from "$lib/types/graph";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    import ContextToggle from "$lib/components/widgets/ContextToggle.svelte";
    import { GamePresenter } from "$lib/presenters/GamePresenter";
    import { KeyValueDisplay } from "$lib/components/ui/key-value-display";
    import { Button } from "$lib/components/ui/button";

    const { data, id } = $props<{ data: NodeData; id: string }>();

    let description;
    $effect(() => {
        description!.innerHTML = steamGameData?.descriptionShort ?? steamGameData?.descriptionLong;
    });

    const gamePresenter = new GamePresenter();
    const steamGameData = $derived(gamePresenter.getSteamGameData(data));

    let selectedContext = $state("steam");

    const platforms = [
        { value: "steam", label: "Steam" },
        { value: "rawg", label: "RAWG" },
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
        alt={steamGameData?.name}
        style="width: 100%; height: 100%; object-fit: cover;"
    />
    <Card.Content>
        <div class="flex flex-col gap-3">
            <KeyValueDisplay label="Name" value={steamGameData?.name} />
            {#if steamGameData?.website}
                <KeyValueDisplay label="Website">
                    <Button
                        variant="link"
                        href={steamGameData.website}
                        class="justify-start p-0"
                        target="_blank"
                    >
                        <span class="truncate">{steamGameData.website}</span>
                    </Button>
                </KeyValueDisplay>
            {/if}

            <!-- <KeyValueDisplay label="App ID" value={steamGameData?.appId} /> -->

            <KeyValueDisplay
                label="Release Date"
                value={steamGameData?.releaseDate}
            />

            <KeyValueDisplay
                label="Publisher"
                value={steamGameData?.publishers.join(", ")}
            />

            <KeyValueDisplay
                label="Developer"
                value={steamGameData?.developers.join(", ")}
            />

            <KeyValueDisplay
                label="Genre"
                value={steamGameData?.genres.join(", ")}
            />

            <KeyValueDisplay
                label="Platforms"
                value={steamGameData?.platforms.join(", ")}
            />

            <KeyValueDisplay
                label="Description"
            >
                <div bind:this={description}></div>
            </KeyValueDisplay>
        </div>
    </Card.Content>
{/snippet}

{#snippet rawgCard()}
    Under Construction!
{/snippet}
