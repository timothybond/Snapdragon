<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import {
        populations,
        selectedPopulation,
        selectedPopulationId,
        selectedGeneration,
    } from "../stores/stores";
    import type { Experiment, Population } from "../types/models";
    import PopulationPanel from "./population/PopulationPanel.svelte";

    export let experiment: Experiment;

    let generation = 0;
    let populationId: string | undefined = $selectedPopulationId;

    $: {
        selectedPopulationId.set(populationId);
    }

    $: {
        selectedGeneration.set(generation);
    }
</script>

<div class="py-5">
    <h1>{experiment.name}</h1>
</div>

<div class="flex pb-5">
    <div class="text-xl">
        <label for="population-dropdown">Population: </label>
        <select
            id="population-dropdown"
            class="border border-black"
            bind:value={populationId}
        >
            <option value={undefined}>[Select]</option>
            {#each $populations as pop}
                <option value={pop.id}>{pop.name}</option>
            {/each}
        </select>
        {#if $selectedPopulation}
            <label for="generation-input">Generation: </label>
            <input
                type="number"
                min={0}
                max={$selectedPopulation.generation}
                bind:value={generation}
                class="border border-black"
            />
        {/if}
    </div>
</div>

{#if $selectedPopulation}
    <PopulationPanel population={$selectedPopulation}></PopulationPanel>
{/if}

<!-- TODO: Fix this ugly hack -->
<div class="population-table" style="display: none" />

<style>
    .population-table {
        border: 1px;
    }
</style>
