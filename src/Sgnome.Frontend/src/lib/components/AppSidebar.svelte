<script lang="ts">
  import * as Sidebar from "$lib/components/ui/sidebar/index.js";
  import { Home, Users, Gamepad2, Library, Settings } from "@lucide/svelte";
</script>
 
<Sidebar.Root collapsible="icon" variant="inset">
  <Sidebar.Header>
    <div class="flex items-center gap-2 px-2">
      <div class="w-8 h-8 rounded-lg bg-primary flex items-center justify-center">
        <span class="text-primary-foreground font-bold text-sm">S</span>
      </div>

    </div>
  </Sidebar.Header>
  
  <Sidebar.Content>

  </Sidebar.Content>
  
  <Sidebar.Footer>
    <div class="p-4">
      <div class="text-xs text-muted-foreground">
        Sgnome v1.0.0
      </div>
    </div>
  </Sidebar.Footer>
</Sidebar.Root>