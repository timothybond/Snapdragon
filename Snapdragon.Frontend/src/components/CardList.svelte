<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import { itemsWithGames, selectedItemId } from "../stores/stores";
    import type { Item, Card } from "../types/models";

    export let cards: Card[];

    const columns: TableColumn<Card>[] = [
        {
            key: "cost",
            title: "Cost",
            value: (c) => c.cost,
            sortable: true,
        },
        {
            key: "name",
            title: "Name",
            value: (c) => c.name,
            sortable: true,
        },
    ];
</script>

<div>
    <SvelteTable
        {columns}
        rows={cards}
        classNameTable="border table-w-auto"
        classNameCell="border px-2"
        classNameThead="bg-slate-300"
    ></SvelteTable>
</div>

<!-- TODO: Fix this ugly hack -->
<div class="item-table" style="display: none" />

<style>
    .item-table {
        border: 1px;
        width: auto;
    }
</style>
