import { writable, derived, type Writable } from "svelte/store";
import {
    type Game,
    type Card,
    type Experiment,
    type Item,
    type Population,
    type ItemWithGames,
    type CardCounts,
} from "../types/models";
import { client } from "../service/client";

export const selectedExperimentId = writable<string | undefined>(undefined);
export const selectedPopulationId = writable<string | undefined>(undefined);
export const selectedGeneration = writable<number>(-1);
export const selectedItemId = writable<string | undefined>(undefined);
export const selectedGameId = writable<string | undefined>(undefined);

export const cards = buildCardStore();

function buildCardStore() {
    let store = writable<Card[]>([]);

    let refresh = () => {
        store.set([]);
        client.getCards().then((cards) => store.set(cards));
    };

    return {
        set: store.set,
        update: store.update,
        subscribe: store.subscribe,
        refresh,
    };
}

export const cardsByName = derived(cards, (cards) => {
    return new Map<string, Card>(cards.map((c) => [c.name, c]));
});

export const populations = buildPopulationStore();

function buildPopulationStore() {
    let store = writable<Population[]>([]);

    let refresh = (experimentId: string) => {
        store.set([]);
        client
            .getPopulations(experimentId)
            .then((populations) => store.set(populations));
    };

    return {
        set: store.set,
        update: store.update,
        subscribe: store.subscribe,
        refresh,
    };
}

export const selectedPopulation = derived(
    [populations, selectedPopulationId],
    ([pops, id]) => {
        if (!pops) {
            return undefined;
        }
        if (!id) {
            return undefined;
        }

        var selectedPop = pops.find((e) => e.id === id);
        return selectedPop;
    }
);

export const experiments = buildExperimentStore();

export const selectedExperiment = derived(
    [experiments, selectedExperimentId],
    ([experiments, id]) => {
        if (!experiments) {
            return undefined;
        }
        if (!id) {
            return undefined;
        }

        var selectedExperiment = experiments.find((e) => e.id === id);

        selectedGeneration.set(-1);
        selectedPopulationId.set("");
        populations.refresh(id);

        return selectedExperiment;
    }
);

function buildExperimentStore(): Writable<Experiment[]> & {
    refresh: () => Promise<void>;
} {
    const store = writable<Experiment[]>([]);

    const refresh = async (): Promise<void> => {
        const experiments = await client.getExperiments();
        store.set(experiments);
    };

    return {
        set: store.set,
        update: store.update,
        subscribe: store.subscribe,
        refresh,
    };
}

export const items = derived(
    [selectedPopulationId, selectedGeneration],
    ([popId, gen], set: (value: Item[]) => void) => {
        set([]);
        if (popId) {
            client.getItems(popId, gen).then((items) => set(items));
        }
    }
);

export const games = derived(
    [selectedExperimentId, selectedGeneration],
    ([experimentId, generation], set: (value: Game[]) => void) => {
        set([]);

        if (experimentId) {
            client
                .getGames(experimentId, generation)
                .then((games) => set(games));
        }
    }
);

export const selectedGame = derived(
    [selectedGameId],
    ([selectedGameId], set: (g: Game | undefined) => void) => {
        set(undefined);

        if (selectedGameId) {
            client.getGame(selectedGameId).then((g) => set(g));
        }
    }
);

export const itemsWithGames = derived([items, games], ([items, games]) => {
    var results: ItemWithGames[] = items.map((i) => {
        return { ...i, games: [], wins: 0 };
    });
    var resultsById = new Map<string, ItemWithGames>(
        results.map((r) => [r.id, r])
    );

    for (var game of games) {
        let topPlayer = resultsById.get(game.topPlayerId);
        if (topPlayer) {
            topPlayer.games.push(game.id);
            if (game.winner === "Top") {
                topPlayer.wins += 1;
            }
        }

        let bottomPlayer = resultsById.get(game.bottomPlayerId);
        if (bottomPlayer) {
            bottomPlayer.games.push(game.id);
            if (game.winner === "Bottom") {
                bottomPlayer.wins += 1;
            }
        }
    }

    return results;
});

export const selectedItem = derived(
    [itemsWithGames, selectedItemId],
    ([items, selectedItemId]) => {
        if (!items) {
            return undefined;
        }
        if (!selectedItemId) {
            return undefined;
        }

        return items.find((i) => i.id == selectedItemId);
    }
);

export const winsPerDeck = derived(games, (g) => {
    let wins = new Map<string, number>();

    for (let game of g) {
        let winner: string | undefined = undefined;
        if (game.winner == "Top") {
            winner = game.topPlayerId;
        } else if (game.winner == "Bottom") {
            winner = game.bottomPlayerId;
        }

        if (winner) {
            wins.set(winner, (wins.get(winner) ?? 0) + 1);
        }
    }

    return wins;
});

// Kinda hacky solution to make sure we don't clean out the hash on load
let onFirstLoad = true;

export const desiredHash = derived(
    [selectedExperimentId, selectedPopulationId],
    ([experimentId, popId]) => {
        let result = "";

        if (experimentId) {
            if (popId) {
                result = `#${experimentId}-${popId}`;
            } else {
                result = `#${experimentId}`;
            }
        }

        if (onFirstLoad) {
            onFirstLoad = false;
        } else {
            window.location.hash = result;
        }

        return result;
    }
);

export const cardCounts = derived(
    selectedPopulationId,
    (selectedPopulationId, set: (cardCounts: CardCounts[]) => void) => {
        set([]);

        if (selectedPopulationId) {
            client
                .getCardCounts(selectedPopulationId)
                .then((cardCounts) => set(cardCounts));
        }
    }
);
