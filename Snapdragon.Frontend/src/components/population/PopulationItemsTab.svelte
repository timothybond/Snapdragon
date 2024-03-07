<script lang="ts">
    import ItemList from "../ItemList.svelte";
    import ItemGameList from "../ItemGameList.svelte";
    import {
        selectedItem,
        cardsByName,
        games,
        selectedGame,
    } from "../../stores/stores";
    import CardList from "../CardList.svelte";
    import type { Card, Game } from "../../types/models";
    import GameLog from "../GameLog.svelte";

    $: cards = $selectedItem?.cards
        .map((n) => $cardsByName.get(n))
        .filter((c) => !!c) as Card[];

    $: itemGames = $selectedItem?.games
        .map((gid) => $games.find((g) => g.id === gid))
        .filter((g) => !!g) as Game[];
</script>

<div class="flex flex-row">
    <div class="flex px-3">
        <ItemList></ItemList>
    </div>
    <div class="flex px-3">
        {#if $selectedItem}
            <CardList {cards}></CardList>
        {/if}
    </div>
    <div class="flex px-3">
        {#if $selectedItem}
            <ItemGameList games={itemGames}></ItemGameList>
        {/if}
    </div>
    <div class="flex px-3">
        {#if $selectedGame}
            <GameLog game={$selectedGame}></GameLog>
        {/if}
    </div>
</div>
