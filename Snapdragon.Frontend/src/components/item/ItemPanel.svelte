<script lang="ts">
    import type { TableColumn } from "svelte-table";
    import { cards } from "../../stores/stores";
    import type { Item, Card } from "../../types/models";
    import SvelteTable from "svelte-table";

    export let item: Item;

    let itemCards = item.cards
        .map((name) => $cards.find((card) => card.name === name))
        .filter((c) => !!c) as Card[];

    const columns: TableColumn<Card>[] = [
        {
            key: "cost",
            title: "Cost",
            value: (c) => c.cost,
            sortable: true,
        },
        { key: "name", title: "Name", value: (c) => c.name, sortable: true },
    ];
</script>

<div class="pt-2 pb-2">
    <SvelteTable
        {columns}
        rows={itemCards}
        classNameTable="population-table border"
        classNameCell="border px-2"
        classNameThead="bg-slate-300"
    ></SvelteTable>
</div>
