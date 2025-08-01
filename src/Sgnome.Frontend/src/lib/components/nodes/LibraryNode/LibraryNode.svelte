<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import { LibraryPresenter } from "$lib/presenters/LibraryPresenter";
    import type { NodeData, Pin } from "$lib/types/graph";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    import GameListWidget from "$lib/components/widgets/GameListWidget.svelte";
    
    const { data, id } = $props<{ data: NodeData, id: string }>();

    const libraryPresenter = new LibraryPresenter();
    const libraryLabel = $derived(libraryPresenter.getLibraryLabel(data));
    const gameListInput = $derived(libraryPresenter.getGameListWidgetInput(data.pins));
</script>

<Card.Root style="width: 480px;">
    <Card.Content>
        <IOHandles hostId={id} />
    </Card.Content>
    <Card.Header>
        <Card.Title>{libraryLabel}</Card.Title>
    </Card.Header>
    <Card.Content>
        <GameListWidget gamePins={gameListInput.gamePins} />
    </Card.Content>
</Card.Root>    