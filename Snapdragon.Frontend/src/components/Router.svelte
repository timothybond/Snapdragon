<script lang="ts">
    import { selectedExperiment } from "../stores/stores";
    import ExperimentPanel from "./ExperimentPanel.svelte";
    import ExperimentList from "./ExperimentList.svelte";
</script>

<div class="h-full">
    {#if !$selectedExperiment}
        <ExperimentList />
    {:else}
        <ExperimentPanel experiment={$selectedExperiment}></ExperimentPanel>
    {/if}
</div>
