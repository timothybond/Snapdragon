<script lang="ts">
    import { selectedExperimentId } from "../stores/stores";

    function goToExperiments() {
        selectedExperimentId.set("");
    }
</script>

<div class="fixed top-0 left-0 w-full border-b bg-white">
    <button on:click={goToExperiments}>Experiments</button>
</div>

<style>
</style>
