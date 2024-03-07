<script lang="ts">
    import { selectedPopulation } from "../../stores/stores";
</script>

<div>
    {#if $selectedPopulation}
        <h2>Fixed Cards</h2>

        <ul>
            {#each $selectedPopulation.fixedCards as fixedCard}
                <li>{fixedCard}</li>
            {/each}
        </ul>

        <h2>All Cards</h2>
        <ul>
            {#each $selectedPopulation.allCards as card}
                <li>{card}</li>
            {/each}
        </ul>
    {/if}
</div>

<style>
</style>
