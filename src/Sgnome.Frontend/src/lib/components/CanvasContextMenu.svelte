<script lang="ts">
    import * as Menubar from "$lib/components/ui/menubar/index.js";
    import { CANVAS_CONTEXT_MENU_EVENTS } from "$lib/constants/canvasContextMenuEvents";
    let {
        open = $bindable(false),
        position = $bindable({ x: 0, y: 0 }),
        onAction = $bindable((action: string) => {}),
    } = $props<{
        open: boolean;
        position: { x: number; y: number };
        onAction: (action: string) => void;
    }>();

    function handleAction(action: string, event?: any) {
        if (event && event.stopPropagation) {
            event.stopPropagation();
        }
        onAction(action);
    }
</script>

{#if open}
    <div
        class="fixed z-[9999]"
        style="left: {position.x}px; top: {position.y}px;"
    >
        <Menubar.Root>
            <Menubar.Menu>
                <Menubar.Trigger onclick={(event) => event.stopPropagation()}>
                    Add
                </Menubar.Trigger>
                <Menubar.Content>
                    <Menubar.Sub>
                        <Menubar.SubTrigger onclick={(event) => event.stopPropagation()}>Node</Menubar.SubTrigger>
                        <Menubar.SubContent>
                            <Menubar.Item onclick={(event) => handleAction(CANVAS_CONTEXT_MENU_EVENTS.ADD_NODE_PLAYER, event)}>Player</Menubar.Item>
                            <Menubar.Item onclick={(event) => handleAction(CANVAS_CONTEXT_MENU_EVENTS.ADD_NODE_GAME, event)}>Game</Menubar.Item>
                        </Menubar.SubContent>
                    </Menubar.Sub>
                </Menubar.Content>
            </Menubar.Menu>

            <Menubar.Menu>
                <Menubar.Trigger onclick={(event) => event.stopPropagation()}>
                    View
                </Menubar.Trigger>
                <Menubar.Content>
                    <Menubar.Item
                        onclick={(event) => handleAction(CANVAS_CONTEXT_MENU_EVENTS.CENTER_VIEW, event)}
                    >
                        Center View
                    </Menubar.Item>
                    <Menubar.Separator />
                    <Menubar.Item
                        onclick={(event) => handleAction(CANVAS_CONTEXT_MENU_EVENTS.FIT_VIEW, event)}
                    >
                        Fit to View
                    </Menubar.Item>
                </Menubar.Content>
            </Menubar.Menu>
        </Menubar.Root>
    </div>
{/if}
