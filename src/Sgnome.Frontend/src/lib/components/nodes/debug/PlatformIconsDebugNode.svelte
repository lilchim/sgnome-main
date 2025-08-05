<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import ExampleUsage from "$lib/components/ui/icons/example-usage.svelte";
    import { Button } from "$lib/components/ui/button";
    import { removeNode } from "$lib/stores/graphState.svelte";
    import type { NodeData } from "$lib/types/graph";

    const { data, id } = $props<{ data: NodeData; id: string }>();
</script>

<Card.Root>
    <Card.Header>
        <Card.Title>Platform Icons Debug</Card.Title>
    </Card.Header>
    <ExampleUsage />
    <Button onclick={() => removeNode(id)}> Close </Button>
</Card.Root>