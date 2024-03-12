<script lang="ts">
    import { getContext } from "svelte";
    import { hiddenCardsForChart } from "../../stores/stores";

    interface CardInPopulation {
        name: string;
        fixed: boolean;
    }

    export let row: CardInPopulation;

    const context: {
        toggleVisibility: (name?: string) => void;
    } = getContext("stats");

    $: checked = !$hiddenCardsForChart.has(row.name);

    function showHideCard(name: string) {
        context.toggleVisibility(name);
    }
</script>

<input type="checkbox" bind:checked on:click={() => showHideCard(row.name)} />
