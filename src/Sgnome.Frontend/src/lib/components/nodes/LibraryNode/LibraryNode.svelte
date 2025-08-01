<script lang="ts">
    import * as Card from "$lib/components/ui/card";
    import { LibraryPresenter } from "$lib/presenters/LibraryPresenter";
    import type { NodeData, Pin } from "$lib/types/graph";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";
    export let data: NodeData;
    export let id: string;

    const libraryPresenter = new LibraryPresenter();
    $: libraryLabel = libraryPresenter.getLibraryLabel(data);

</script>

<Card.Root class="w-80">
    <Card.Content>
        <IOHandles hostId={id} />
    </Card.Content>
    <Card.Header>
        <Card.Title>{libraryLabel}</Card.Title>
    </Card.Header>
</Card.Root>