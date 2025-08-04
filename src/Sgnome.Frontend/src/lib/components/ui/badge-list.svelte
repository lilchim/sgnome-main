<script lang="ts">
    import { Badge } from "$lib/components/ui/badge";
    
    let {
        items = $bindable([]),
        variant = $bindable("secondary" as "default" | "secondary" | "destructive" | "outline"),
        class: className = $bindable("")
    } = $props<{
        items: string[];
        variant?: "default" | "secondary" | "destructive" | "outline";
        class?: string;
    }>();
</script>

<div class="flex flex-wrap gap-1 {className}">
    {#each items as item}
        <Badge variant={variant} class="text-xs">
            {item}
        </Badge>
    {/each}
</div> 