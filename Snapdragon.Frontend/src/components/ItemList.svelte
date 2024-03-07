<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import {
        itemsWithGames,
        selectedGameId,
        selectedItemId,
    } from "../stores/stores";
    import type { Item, ItemWithGames } from "../types/models";

    let selectedKeys: (string | number)[];

    const columns: TableColumn<ItemWithGames>[] = [
        {
            key: "id",
            title: "ID",
            value: (e) => e.id,
            sortable: true,
        },
        { key: "wins", title: "Wins", value: (e) => e.wins, sortable: true },
    ];

    $: {
        if (selectedKeys) {
            if ($selectedItemId !== selectedKeys[0]?.toString()) {
                selectedGameId.set(undefined);
                selectedItemId.set(selectedKeys[0]?.toString());
            }
        }
    }
</script>

<div>
    <SvelteTable
        {columns}
        rows={$itemsWithGames}
        classNameTable="border table-w-auto"
        classNameThead="bg-slate-300"
        classNameRowSelected="bg-blue-100"
        classNameCell="border px-2"
        selectOnClick={true}
        selectSingle={true}
        rowKey="id"
        bind:selected={selectedKeys}
    ></SvelteTable>
</div>

<style>
</style>
