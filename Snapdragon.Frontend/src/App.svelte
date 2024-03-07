<script lang="ts">
    import svelteLogo from "./assets/svelte.svg";
    import viteLogo from "/vite.svg";
    import Counter from "./lib/Counter.svelte";
    import TailwindCss from "./lib/TailwindCSS.svelte";
    import Navbar from "./components/Navbar.svelte";
    import Router from "./components/Router.svelte";
    import { onMount } from "svelte";
    import {
        cards,
        desiredHash,
        experiments,
        selectedExperimentId,
        selectedPopulation,
        selectedPopulationId,
    } from "./stores/stores";

    onMount(() => {
        const hash = window.location.hash;

        cards.refresh();

        experiments.refresh().then(() => {
            if (!hash || hash === "#") {
                selectedExperimentId.set("");
                return;
            }

            var experimentId = hash.substring(1, 37);
            selectedExperimentId.set(experimentId);

            if (hash.length > 49) {
                selectedPopulationId.set(hash.substring(38, 74));
            }

            // This mostly doesn't matter, except we need somebody to subscribe to this
            window.location.hash = $desiredHash;
        });
    });
</script>

<TailwindCss />

<main class="box-border h-full m-0 p-0 mx-auto text-left">
    <div class="h-12">
        <Navbar />
    </div>
    <div class="p-0 m-0">
        <Router />
    </div>
</main>

<style>
</style>
