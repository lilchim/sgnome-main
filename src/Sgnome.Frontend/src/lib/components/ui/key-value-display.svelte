<!--
KeyValueDisplay Component

A reusable component for displaying key-value pairs with consistent styling.
Supports both simple text values and custom content using Svelte 5's @render.

Usage:
1. Simple text value:
   <KeyValueDisplay label="Name" value="Counter-Strike 2" />

2. Custom content (links, badges, etc.):
   <KeyValueDisplay label="Profile URL">
       <Button variant="link" href={url}>View Profile</Button>
   </KeyValueDisplay>

3. Status with badge:
   <KeyValueDisplay label="Status">
       <Badge variant="default">{status}</Badge>
   </KeyValueDisplay>
-->

<script lang="ts">
    import type { Snippet } from 'svelte';
    
    let {
        label = $bindable(""),
        value = $bindable(""),
        class: className = $bindable(""),
        children = $bindable(null as Snippet | null)
    } = $props<{
        label: string;
        value?: string;
        class?: string;
        children?: Snippet;
    }>();
</script>

<div class="flex flex-col {className}">
    <span class="text-sm font-medium text-muted-foreground">{label}:</span>
    {#if children}
        {@render children()}
    {:else}
        <span class="text-sm">{value || 'N/A'}</span>
    {/if}
</div> 