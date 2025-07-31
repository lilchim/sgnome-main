<script lang="ts">
    import type { NodeData } from "../../../types/graph";
    import { PinBehavior, PinState } from "../../../types/graph";
    import { PlayerPresenter } from "../../../presenters/PlayerPresenter";
    import * as Card from "$lib/components/ui/card/index.js";
    import { Separator } from "../../ui/separator";
    import * as Accordion from "../../ui/accordion";
    import { Handle, Position } from "@xyflow/svelte";

    export let data: NodeData;
    export let id: string; // The node ID from SvelteFlow

    const presenter = new PlayerPresenter();

    // Extract player data from the node data properties

    $: displayName = presenter.getDisplayName(data);
    $: libraryCount = presenter.getLibraryCount(data);

    // $: playerData = (data?.properties as any) || {};
    // // $: displayName = (playerData.displayName as string) || "Unknown Player";
    $: avatarUrl = presenter.getAvatarUrl(data);
    $: profilesBySource = presenter.getProfilesBySource(data);
    $: availableProfileSources = presenter.getAvailableProfileSources(data);
    $: internalId = data.properties.InternalId as string | undefined;
    // $: identifiers = playerData.identifiers as Record<string, string> | undefined;

    $: libraryPins = presenter.getLibraryPins(data);
</script>

<Card.Root class="w-80">
    <Card.Header class="pb-2">
        <div class="flex items-center gap-3">
            {#if avatarUrl}
                <img
                    src={avatarUrl}
                    alt={displayName}
                    class="w-8 h-8 rounded-full object-cover"
                />
            {:else}
                <div
                    class="w-8 h-8 rounded-full bg-muted flex items-center justify-center"
                >
                    <span class="text-xs font-medium text-muted-foreground">
                        {displayName.charAt(0).toUpperCase()}
                    </span>
                </div>
            {/if}
            <div class="flex-1 min-w-0">
                <Card.Title class="text-sm font-medium truncate">
                    {displayName}
                </Card.Title>
                {#if internalId}
                    <Card.Description class="text-xs text-muted-foreground">
                        ID: {internalId}
                    </Card.Description>
                {/if}
            </div>
        </div>
    </Card.Header>
    <Separator />

    <!-- Player Profiles -->
    <Card.Content class="pt-0">
        <Accordion.Root type="single">
            <Accordion.Item value="item-1">
                <Accordion.Trigger>Profiles</Accordion.Trigger>
                <Accordion.Content>
                    <Accordion.Root type="multiple">
                        {#each availableProfileSources as source}
                            <Accordion.Item value={source}>
                                <Accordion.Trigger>
                                    {source}
                                </Accordion.Trigger>
                                <Accordion.Content>
                                    {#each profilesBySource[source] as pin}
                                        <div>
                                            {pin.label}
                                            {pin.summary.displayText}
                                        </div>
                                    {/each}
                                </Accordion.Content>
                            </Accordion.Item>
                        {/each}
                    </Accordion.Root>
                </Accordion.Content>
            </Accordion.Item>
        </Accordion.Root>
    </Card.Content>

    <Separator />

    <Card.Content class="pt-0">
        <div>
            {#each libraryPins as pin}
                <div class="flex items-center justify-between">
                    <span>{pin.summary.displayText}</span>
                    <Handle
                        type="source"
                        position={Position.Right}
                        id={pin.id}
                        class="pin-handle"
                    />
                </div>
            {/each}
        </div>
    </Card.Content>
</Card.Root>
