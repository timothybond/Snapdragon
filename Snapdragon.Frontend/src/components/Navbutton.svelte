<script lang="ts">
    export let text: string;
    export let hash: string;

    function goToHash() {
        window.location.hash = `#${hash}`;
    }
</script>

<button type="button" on:click={goToHash}>
    {text}
</button>
