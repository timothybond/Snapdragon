export interface Card {
    name: string;
    cost: number;
    power: number;
}

export interface Experiment {
    id: string;
    name: string;
    started: string;
}

export interface GameLog {
    turn: number;
    order: number;
    contents: string;
}

export interface Game {
    id: string;
    topPlayerId: string;
    bottomPlayerId: string;
    winner: string;
    experimentId: string | undefined;
    generation: number | undefined;
    logs: GameLog[] | undefined;
}

export interface Item {
    id: string;
    firstParentId: string | undefined;
    secondParentId: string | undefined;
    cards: string[];
}

export type ItemWithGames = Item & {
    games: string[];
    wins: number;
};

export interface Population {
    id: string;
    name: string;
    generation: number;
    fixedCards: string[];
    allCards: string[];
}

export interface CardCounts {
    name: string;
    counts: number[];
}
