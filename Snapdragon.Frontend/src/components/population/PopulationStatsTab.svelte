<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import {
        cardCounts,
        hiddenCardsForChart,
        populations,
        selectedPopulation,
    } from "../../stores/stores";
    import type { Card, Population } from "../../types/models";
    import CardCountChart from "../CardCountChart.svelte";
    import CardVisibilityToggle from "./CardVisibilityToggle.svelte";
    import { setContext } from "svelte";

    interface CardInPopulation {
        name: string;
        fixed: boolean;
    }

    $: allShown = $hiddenCardsForChart.size == 0;
    $: anyHidden = $hiddenCardsForChart.size > 0;
    $: anyShown =
        $hiddenCardsForChart.size < ($selectedPopulation?.allCards.length ?? 0);

    function toggleVisibility(name?: string) {
        if (name) {
            if ($hiddenCardsForChart.has(name)) {
                hiddenCardsForChart.remove(name);
            } else {
                hiddenCardsForChart.add(name);
            }
        }
    }

    function toggleAll() {
        if (anyHidden) {
            hiddenCardsForChart.clear();
        } else {
            for (const name of $selectedPopulation?.allCards ?? []) {
                hiddenCardsForChart.add(name);
            }
        }
    }

    setContext("stats", {
        toggleVisibility,
    });

    function getCardsInPopulation(pop: Population): CardInPopulation[] {
        let fixedCards = new Set<string>(pop.fixedCards);

        return pop.allCards.map((name) => {
            return {
                name: name,
                fixed: fixedCards.has(name),
            };
        });
    }

    $: cards = $selectedPopulation
        ? getCardsInPopulation($selectedPopulation)
        : [];

    const columns: TableColumn<CardInPopulation>[] = [
        {
            key: "shown",
            title: " ",
            renderComponent: {
                component: CardVisibilityToggle,
            },
        },
        {
            key: "name",
            title: "Name",
            value: (card) => (card.fixed ? `*${card.name}` : card.name),
        },
    ];
</script>

<div class="flex">
    <div class="flex-none">
        <SvelteTable
            {columns}
            rows={cards}
            classNameTable="border table-w-auto"
            classNameCell="border px-2"
            classNameThead="bg-slate-300"
        >
            <svelte:fragment slot="header">
                <tr class="bg-slate-300">
                    <th class="px-2"
                        ><input
                            type="checkbox"
                            indeterminate={anyShown && anyHidden}
                            bind:checked={allShown}
                            on:click={toggleAll}
                        /></th
                    >
                    <th class="px-2">Name</th>
                </tr>
            </svelte:fragment>
        </SvelteTable>
    </div>
    <div class="flex-1">
        <CardCountChart cardCounts={$cardCounts}></CardCountChart>
    </div>
</div>
