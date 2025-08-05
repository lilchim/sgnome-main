<!-- Example usage of platform icons -->

<script lang="ts">
    import { SteamIcon, EpicIcon, XboxIcon, PlayStationIcon } from "./index";
    import { Badge } from "$lib/components/ui/badge";
    import { Button } from "$lib/components/ui/button";
    import { KeyValueDisplay } from "$lib/components/ui/key-value-display";
    
    // Example data
    const platforms = [
        { name: "Steam", icon: SteamIcon, color: "text-blue-600", status: "connected" },
        { name: "Epic", icon: EpicIcon, color: "text-purple-600", status: "not-connected" },
        { name: "Xbox", icon: XboxIcon, color: "text-green-600", status: "connected" },
        { name: "PlayStation", icon: PlayStationIcon, color: "text-blue-500", status: "not-connected" }
    ];
</script>

<div class="space-y-6 p-4">
    <h2 class="text-xl font-bold">Platform Icons Examples</h2>
    
    <!-- Basic icon usage -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Basic Icons</h3>
        <div class="flex items-center gap-4">
            {#each platforms as platform}
                <div class="flex items-center gap-2">
                    <svelte:component this={platform.icon} size={24} class={platform.color} />
                    <span class="text-sm">{platform.name}</span>
                </div>
            {/each}
        </div>
    </div>
    
    <!-- Icons with badges -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Icons with Status Badges</h3>
        <div class="flex items-center gap-4">
            {#each platforms as platform}
                <div class="flex items-center gap-2">
                    <svelte:component this={platform.icon} size={20} class={platform.color} />
                    <Badge variant={platform.status === 'connected' ? 'default' : 'secondary'}>
                        {platform.status === 'connected' ? 'Connected' : 'Not Connected'}
                    </Badge>
                </div>
            {/each}
        </div>
    </div>
    
    <!-- Icons in buttons -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Icons in Buttons</h3>
        <div class="flex items-center gap-2">
            {#each platforms as platform}
                <Button variant="outline" size="sm" class="flex items-center gap-2">
                    <svelte:component this={platform.icon} size={16} class={platform.color} />
                    Connect {platform.name}
                </Button>
            {/each}
        </div>
    </div>
    
    <!-- Icons in key-value displays -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Icons in KeyValueDisplay</h3>
        <div class="space-y-3">
            {#each platforms as platform}
                <KeyValueDisplay label={platform.name}>
                    <div class="flex items-center gap-2">
                        <svelte:component this={platform.icon} size={16} class={platform.color} />
                        <span class="text-sm">
                            {platform.status === 'connected' ? 'Connected' : 'Not connected'}
                        </span>
                    </div>
                </KeyValueDisplay>
            {/each}
        </div>
    </div>
    
    <!-- Different sizes -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Different Sizes</h3>
        <div class="flex items-center gap-4">
            <SteamIcon size={16} class="text-blue-600" />
            <SteamIcon size={20} class="text-blue-600" />
            <SteamIcon size={24} class="text-blue-600" />
            <SteamIcon size={32} class="text-blue-600" />
            <SteamIcon size={48} class="text-blue-600" />
        </div>
    </div>
    
    <!-- Custom colors -->
    <div class="space-y-2">
        <h3 class="text-lg font-semibold">Custom Colors</h3>
        <div class="flex items-center gap-4">
            <SteamIcon size={24} class="text-blue-600" />
            <SteamIcon size={24} class="text-gray-600" />
            <SteamIcon size={24} class="text-red-600" />
            <SteamIcon size={24} class="text-green-600" />
            <SteamIcon size={24} class="text-purple-600" />
        </div>
    </div>
</div> 