<script lang="ts">
    import Tabs from "../common/Tabs.svelte";
    import type { TabItem } from "../../lib/TabItem";
    import PopulationCardTab from "./PopulationCardTab.svelte";
    import PopulationItemsTab from "./PopulationItemsTab.svelte";
    import PopulationStatsTab from "./PopulationStatsTab.svelte";

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
        {
            label: "Statistics",
            value: 3,
            component: PopulationStatsTab,
        },
    ];
</script>

<Tabs items={tabItems}></Tabs>
