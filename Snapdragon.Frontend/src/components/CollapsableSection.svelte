<script lang="ts">
    export let collapsed: boolean = true;
    export let title: string;

    function toggle() {
        collapsed = !collapsed;
    }
</script>

<div>
    <button on:click={toggle}><h1>{collapsed ? "+" : "-"} {title}</h1></button>
    {#if !collapsed}
        <div>
            <slot />
        </div>
    {/if}
</div>

<style>
    button {
        background-color: transparent;
        border: 0;
    }

    button:active {
        border: 0;
    }
</style>
