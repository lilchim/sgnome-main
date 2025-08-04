<script lang="ts">
    import * as Card from "$lib/components/ui/card/index.js";
    import { Button } from "$lib/components/ui/button";
    import { Separator } from "$lib/components/ui/separator";
    import type { NodeData } from "$lib/types/graph";
    import { removeNode } from "$lib/stores/graphState.svelte";
    import IOHandles from "$lib/components/widgets/IOHandles.svelte";

    const { data, id } = $props<{ data: NodeData; id: string }>();

    function handleDismiss() {
        removeNode(id);
    }
</script>

<Card.Root class="w-96">
    <Card.Header>
        <Card.Title class="text-lg">Welcome to Sgnome! 🎮</Card.Title>
        <Card.Description>
            Progressive Game Discovery
        </Card.Description>
    </Card.Header>
    
    <Card.Content class="space-y-4">
        <div class="space-y-3">
            <div class="bg-blue-50 dark:bg-blue-950/20 p-3 rounded-lg border border-blue-200 dark:border-blue-800">
                <p class="text-sm text-blue-800 dark:text-blue-200 font-medium mb-1">
                    ⚠️ Early Prototype
                </p>
                <p class="text-xs text-blue-700 dark:text-blue-300">
                    This app is in very early development and updating frequently. 
                    Features may change or break without notice.
                </p>
            </div>

            <div class="space-y-2">
                <h4 class="text-sm font-medium">What is This?</h4>
                <p class="text-sm text-muted-foreground">
                    Sgnome helps you discover games and media progressively. Start with what you know 
                    and explore connections to find new content across different platforms and sources.
                </p>
            </div>

            <Separator />

            <div class="space-y-2">
                <h4 class="text-sm font-medium">How to get started:</h4>
                <div class="space-y-2 text-sm text-muted-foreground">
                    <div class="flex items-start gap-2">
                        <span class="text-primary font-medium">1.</span>
                        <span>Right-click anywhere on the canvas to open the context menu</span>
                    </div>
                    <div class="flex items-start gap-2">
                        <span class="text-primary font-medium">2.</span>
                        <span>Select "Add" → "Node" → "Player" to add a player node</span>
                    </div>
                    <div class="flex items-start gap-2">
                        <span class="text-primary font-medium">3.</span>
                        <span>If you see a "handle", you can drag it to expand to a new Node</span>
                    </div>
                    <div class="flex items-start gap-2">
                        <span class="text-primary font-medium">4.</span>
                        <span>Explore the data and discover new connections!</span>
                    </div>
                </div>
            </div>

            <Separator />

            <div class="space-y-2">
                <h4 class="text-sm font-medium">Current Features:</h4>
                <ul class="text-sm text-muted-foreground space-y-1">
                    <li>• Add and explore player profiles</li>
                    <li>• View game libraries and collections</li>
                    <li>• Steam API Integration with more platforms coming</li>
                    <li>• Progressive data discovery</li>
                </ul>
            </div>
        </div>
    </Card.Content>

    <Card.Footer>
        <Button 
            onclick={handleDismiss}
            class="w-full"
            variant="outline"
        >
            Close This 
        </Button>
    </Card.Footer>
</Card.Root> 