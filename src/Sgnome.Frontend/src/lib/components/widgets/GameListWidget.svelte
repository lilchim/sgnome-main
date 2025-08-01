<script lang="ts">
    import type { Pin } from "$lib/types/graph";

    export let input: { gamePins: Pin[] };
</script>

<div>
    {#each input.gamePins as gamePin}
        <div>
            <h1>{gamePin.label}</h1>
        </div>
    {/each}
</div>