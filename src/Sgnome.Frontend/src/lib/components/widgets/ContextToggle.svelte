<script lang="ts">
    import * as ToggleGroup from "$lib/components/ui/toggle-group";
    import type { Snippet } from 'svelte';

    let {
        contexts = $bindable([]),
        selectedContext = $bindable(""),
        onContextChange = $bindable((context: string) => {}),
        renderContexts = $bindable({} as Record<string, Snippet>)
    } = $props<{
        contexts: Array<{ value: string; label: string }>;
        selectedContext: string;
        onContextChange: (context: string) => void;
        renderContexts?: Record<string, Snippet>;
    }>();

    function handleContextChange(value: string) {
        selectedContext = value;
        onContextChange(value);
    }
</script>

<div style="position: absolute; top: -18px; right: 16px;">
    <ToggleGroup.Root
        type="single"
        value={selectedContext}
        onValueChange={handleContextChange}
    >
        {#each contexts as context}
            <ToggleGroup.Item value={context.value}>
                {#if renderContexts[context.value]}
                    {@render renderContexts[context.value]()}
                {:else}
                    {context.label}
                {/if}
            </ToggleGroup.Item>
        {/each}
    </ToggleGroup.Root>
</div> 