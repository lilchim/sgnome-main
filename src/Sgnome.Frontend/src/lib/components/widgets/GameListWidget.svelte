<script lang="ts">
    import type { Pin } from "$lib/types/graph";
    import { Handle, Position } from "@xyflow/svelte";
    import { Separator } from "$lib/components/ui/separator";
    import { Button } from "$lib/components/ui/button";
    import { ListPlusIcon, ListXIcon } from "@lucide/svelte";
    import { Input } from "$lib/components/ui/input/index.js";
    import { ScrollArea } from "$lib/components/ui/scroll-area";

    const { gamePins } = $props<{ gamePins: Pin[] }>();

    let selectedGames = $state<Pin[]>([]);
    let searchText = $state<string>("");

    const unselectedGames = $derived(
        gamePins.filter((gamePin: Pin) => !selectedGames.includes(gamePin)),
    );

    const filteredUnselectedGames = $derived(() => {
        if (!searchText.trim()) {
            return unselectedGames;
        }

        const searchLower = searchText.toLowerCase();
        return unselectedGames.filter((gamePin: Pin) =>
            gamePin.label.toLowerCase().includes(searchLower),
        );
    });

    // Prevent scroll events from propagating to XYFlow, otherwise it will zoom
    function handleScrollAreaWheel(event: WheelEvent) {
        event.stopPropagation();
    }
</script>

<div class="flex flex-col gap-2">
    <div>
        <h1>Selected Games</h1>
        {#each selectedGames as gamePin}
            <div>
                <div class="flex justify-between">
                    <Button
                        size="sm"
                        variant="ghost"
                        onclick={() => {
                            selectedGames = selectedGames.filter(
                                (g) => g.id !== gamePin.id,
                            );
                        }}
                    >
                        <ListXIcon />
                    </Button>
                    <h1>{gamePin.label}</h1>
                    <Handle
                        id={gamePin.id}
                        type="source"
                        position={Position.Right}
                        class="pin-handle"
                    />
                </div>
            </div>
        {/each}
    </div>
    <Separator />
    <div class="flex flex-col gap-2">
        <h1>Available Games</h1>
        <div class="flex w-full max-w-sm flex-col gap-1.5">
            <Input
                type="text"
                id="search"
                placeholder="Filter by Name"
                bind:value={searchText}
            />
        </div>
        <ScrollArea class="h-[320px] rounded-md border" onwheel={handleScrollAreaWheel}>
            {#each filteredUnselectedGames() as gamePin}
                <div class="flex items-center p-2">
                    <Button
                        size="icon"
                        variant="ghost"
                        onclick={() => {
                            selectedGames.push(gamePin);
                        }}
                    >
                        <ListPlusIcon />
                    </Button>
                    <span class="text-md">{gamePin.label}</span>
                </div>
            {/each}
        </ScrollArea>
    </div>
</div>
