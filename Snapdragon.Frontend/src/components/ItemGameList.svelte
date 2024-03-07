<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import { selectedItem, selectedGameId } from "../stores/stores";
    import type { Game } from "../types/models";

    export let games: Game[];

    let selectedKeys: (string | number)[];

    const columns: TableColumn<Game>[] = [
        {
            key: "id",
            title: "ID",
            value: (g) => g.id,
            sortable: true,
        },
        {
            key: "opponent",
            title: "Opponent",
            value: (g) =>
                g.topPlayerId === $selectedItem?.id
                    ? g.bottomPlayerId
                    : g.topPlayerId,
            sortable: true,
        },
        {
            key: "outcome",
            title: "Outcome",
            value: (g) => {
                if (g.topPlayerId === $selectedItem?.id && g.winner === "Top") {
                    return "Win";
                }
                if (
                    g.bottomPlayerId === $selectedItem?.id &&
                    g.winner === "Bottom"
                ) {
                    return "Win";
                }
                return "Loss";
            },
        },
    ];

    $: {
        if (selectedKeys) {
            selectedGameId.set(selectedKeys[0]?.toString());
        }
    }
</script>

<div>
    <SvelteTable
        {columns}
        rows={games}
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
