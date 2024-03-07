<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import {
        experiments,
        selectedExperiment,
        populations,
        selectedPopulation,
        selectedPopulationId,
        selectedGeneration,
        items,
    } from "../../stores/stores";
    import type { Experiment, Population } from "../../types/models";
    import CollapsableSection from "../CollapsableSection.svelte";
    import ItemPanel from "../item/ItemPanel.svelte";
    import ItemList from "../ItemList.svelte";
    import Tabs from "../common/Tabs.svelte";
    import type { TabItem } from "../../lib/TabItem";
    import PopulationCardTab from "./PopulationCardTab.svelte";
    import PopulationItemsTab from "./PopulationItemsTab.svelte";

    export let population: Population;

    let tabItems: TabItem[] = [
        {
            label: "Items",
            value: 1,
            component: PopulationItemsTab,
        },
        {
            label: "Cards",
            value: 2,
            component: PopulationCardTab,
        },
    ];
</script>

<Tabs items={tabItems}></Tabs>
