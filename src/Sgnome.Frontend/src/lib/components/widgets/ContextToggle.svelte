<script lang="ts">
    import * as ToggleGroup from "$lib/components/ui/toggle-group";

    let {
        contexts = $bindable([]),
        selectedContext = $bindable(""),
        onContextChange = $bindable((context: string) => {})
    } = $props<{
        contexts: Array<{ value: string; label: string }>;
        selectedContext: string;
        onContextChange: (context: string) => void;
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
            <ToggleGroup.Item value={context.value}>{context.label}</ToggleGroup.Item>
        {/each}
    </ToggleGroup.Root>
</div> 