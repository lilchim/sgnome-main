<script lang="ts">
    import type { Pin } from "$lib/types/graph";
    import { Handle, Position } from "@xyflow/svelte";
    import { Separator } from "$lib/components/ui/separator";
    import { Button } from "$lib/components/ui/button";
    import { PlusIcon, XIcon } from "@lucide/svelte";
    import { Input } from "$lib/components/ui/input/index.js";
    import { ScrollArea } from "$lib/components/ui/scroll-area";
    import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "$lib/components/ui/table";
    import * as Pagination from "$lib/components/ui/pagination/index.js";

    const { gamePins } = $props<{ gamePins: Pin[] }>();

    let selectedGames = $state<Pin[]>([]);
    let searchText = $state<string>("");
    let currentPage = $state(1);
    let sortField = $state<'name' | 'playtime'>('name');
    let sortDirection = $state<'asc' | 'desc'>('asc');
    const perPage = 5;

    const unselectedGames = $derived(
        gamePins.filter((gamePin: Pin) => !selectedGames.includes(gamePin)),
    );

    const filteredUnselectedGames = $derived(() => {
        let filtered = unselectedGames;
        
        if (searchText.trim()) {
            const searchLower = searchText.toLowerCase();
            filtered = unselectedGames.filter((gamePin: Pin) =>
                gamePin.label.toLowerCase().includes(searchLower),
            );
        }

        // Sort the filtered results
        return filtered.sort((a: Pin, b: Pin) => {
            let aValue: string | number;
            let bValue: string | number;

            if (sortField === 'name') {
                aValue = a.label.toLowerCase();
                bValue = b.label.toLowerCase();
            } else {
                // playtime sorting
                aValue = (a.summary?.preview?.["playtime-forever"] as number) || 0;
                bValue = (b.summary?.preview?.["playtime-forever"] as number) || 0;
            }

            if (sortDirection === 'asc') {
                return aValue < bValue ? -1 : aValue > bValue ? 1 : 0;
            } else {
                return aValue > bValue ? -1 : aValue < bValue ? 1 : 0;
            }
        });
    });

    const paginatedGames = $derived(() => {
        const startIndex = (currentPage - 1) * perPage;
        const endIndex = startIndex + perPage;
        return filteredUnselectedGames().slice(startIndex, endIndex);
    });

    // Reset to first page when search changes
    $effect(() => {
        searchText;
        currentPage = 1;
    });

    // Prevent scroll events from propagating to XYFlow, otherwise it will zoom
    function handleScrollAreaWheel(event: WheelEvent) {
        event.stopPropagation();
    }

    function truncateText(text: string, maxLength: number = 30): string {
        if (text.length <= maxLength) {
            return text;
        }
        return text.slice(0, maxLength) + '...';
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
                        <XIcon />
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
        <div class="rounded-md border">
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead class="w-12 flex-shrink-0">Select</TableHead>
                        <TableHead 
                            class="flex-1 min-w-0 cursor-pointer hover:bg-muted/50" 
                            onclick={() => {
                                if (sortField === 'name') {
                                    sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
                                } else {
                                    sortField = 'name';
                                    sortDirection = 'asc';
                                }
                            }}
                        >
                            Name {sortField === 'name' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                        </TableHead>
                        <TableHead 
                            class="w-12 flex-shrink-0 cursor-pointer hover:bg-muted/50"
                            onclick={() => {
                                if (sortField === 'playtime') {
                                    sortDirection = sortDirection === 'asc' ? 'desc' : 'asc';
                                } else {
                                    sortField = 'playtime';
                                    sortDirection = 'asc';
                                }
                            }}
                        >
                            Playtime {sortField === 'playtime' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                        </TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {#each paginatedGames() as gamePin}
                        <TableRow>
                            <TableCell class="flex-shrink-0">
                                <Button
                                    size="icon"
                                    variant="ghost"
                                    onclick={() => {
                                        selectedGames.push(gamePin);
                                    }}
                                >
                                    <PlusIcon />
                                </Button>
                            </TableCell>
                            <TableCell class="min-w-0 w-0">
                                <div title={gamePin.label}>{truncateText(gamePin.label)}</div>
                            </TableCell>
                            <TableCell class="text-muted-foreground flex-shrink-0">{Math.round(gamePin.summary.preview["playtime-forever"] / 3600)}h</TableCell>
                        </TableRow>
                    {/each}
                </TableBody>
            </Table>
        </div>
        {#if filteredUnselectedGames().length > perPage}
            <Pagination.Root count={filteredUnselectedGames().length} {perPage} bind:page={currentPage}>
                {#snippet children({ pages, currentPage: paginationCurrentPage })}
                    <Pagination.Content>
                        <Pagination.Item>
                            <Pagination.PrevButton>
                                Previous
                            </Pagination.PrevButton>
                        </Pagination.Item>
                        {#each pages as page (page.key)}
                            {#if page.type === "ellipsis"}
                                <Pagination.Item>
                                    <Pagination.Ellipsis />
                                </Pagination.Item>
                            {:else}
                                <Pagination.Item>
                                    <Pagination.Link {page} isActive={paginationCurrentPage === page.value}>
                                        {page.value}
                                    </Pagination.Link>
                                </Pagination.Item>
                            {/if}
                        {/each}
                        <Pagination.Item>
                            <Pagination.NextButton>
                                Next
                            </Pagination.NextButton>
                        </Pagination.Item>
                    </Pagination.Content>
                {/snippet}
            </Pagination.Root>
        {/if}
    </div>
</div>
