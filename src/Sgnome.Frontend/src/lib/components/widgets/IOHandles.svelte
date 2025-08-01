<script lang="ts">
    import { Handle, Position } from "@xyflow/svelte";

    export let hostId: string;
</script>

<div class="flex justify-between">
    <Handle
        type="target"
        position={Position.Left}
        id={`${hostId}-input`}
        class="pin-handle"
    />
    <Handle
        type="source"
        position={Position.Right}
        id={`${hostId}-output`}
        class="pin-handle"
    />
</div>
