<!-- Example usage of KeyValueDisplay component -->

<script lang="ts">
    import { KeyValueDisplay } from "./index";
    import { BadgeList } from "./index";
    import { Badge } from "$lib/components/ui/badge";
    import { Button } from "$lib/components/ui/button";
    
    // Example data
    const gameData = {
        name: "Counter-Strike 2",
        appId: "730",
        releaseDate: "2023-09-27",
        developer: "Valve",
        publisher: "Valve",
        genres: ["FPS", "Shooter", "Action"],
        tags: ["Multiplayer", "Competitive", "Tactical"],
        status: "released",
        website: "https://store.steampowered.com/app/730"
    };
    
    const playerData = {
        name: "Player123",
        steamId: "76561198012345678",
        status: "online",
        profileUrl: "https://steamcommunity.com/id/player123",
        createdAt: "2020-01-15"
    };
</script>

<div class="space-y-1">
    <h3 class="text-lg font-semibold">Game Information</h3>
    
    <!-- Simple key-value pairs -->
    <KeyValueDisplay label="Name" value={gameData.name} />
    <KeyValueDisplay label="App ID" value={gameData.appId} />
    <KeyValueDisplay label="Release Date" value={gameData.releaseDate} />
    <KeyValueDisplay label="Developer" value={gameData.developer} />
    <KeyValueDisplay label="Publisher" value={gameData.publisher} />
    
    <!-- Custom value with link -->
    <KeyValueDisplay label="Website">
        <Button variant="link" href={gameData.website} class="justify-start p-0">
            Visit Store Page
        </Button>
    </KeyValueDisplay>
    
    <!-- Status with badge -->
    <KeyValueDisplay label="Status">
        <Badge variant={gameData.status === 'released' ? 'default' : 'secondary'}>
            {gameData.status}
        </Badge>
    </KeyValueDisplay>
    
    <!-- Lists with badges -->
    <KeyValueDisplay label="Genres">
        <BadgeList items={gameData.genres} variant="default" />
    </KeyValueDisplay>
    
    <KeyValueDisplay label="Tags">
        <BadgeList items={gameData.tags} variant="secondary" />
    </KeyValueDisplay>
    
    <hr class="my-4" />
    
    <h3 class="text-lg font-semibold">Player Information</h3>
    
    <KeyValueDisplay label="Name" value={playerData.name} />
    <KeyValueDisplay label="Steam ID" value={playerData.steamId} />
    <KeyValueDisplay label="Created" value={playerData.createdAt} />
    
    <!-- Custom value with link -->
    <KeyValueDisplay label="Profile">
        <Button variant="link" href={playerData.profileUrl} class="justify-start p-0">
            View Steam Profile
        </Button>
    </KeyValueDisplay>
    
    <!-- Status with badge -->
    <KeyValueDisplay label="Status">
        <Badge variant={playerData.status === 'online' ? 'default' : 'secondary'}>
            {playerData.status}
        </Badge>
    </KeyValueDisplay>
</div> 