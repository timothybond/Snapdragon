<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import { itemsWithGames, selectedItemId } from "../stores/stores";
    import type { Item, Game, GameLog } from "../types/models";

    export let game: Game;

    const columns: TableColumn<GameLog>[] = [
        {
            key: "order",
            title: "Order",
            value: (c) => c.order,
            sortable: true,
        },
        {
            key: "turn",
            title: "Turn",
            value: (c) => c.turn,
            sortable: false,
        },
        {
            key: "contents",
            title: "Contents",
            value: (c) => c.contents,
            sortable: true,
        },
    ];
</script>

<div>
    {#if game?.logs}
        <SvelteTable
            {columns}
            rows={game.logs}
            classNameTable="border table-w-auto"
            classNameCell="border px-2 whitespace-pre font-mono"
            classNameThead="bg-slate-300"
        ></SvelteTable>
    {/if}
</div>

<!-- TODO: Fix this ugly hack -->
<div class="item-table" style="display: none" />

<style>
    .item-table {
        border: 1px;
        width: auto;
    }
</style>
