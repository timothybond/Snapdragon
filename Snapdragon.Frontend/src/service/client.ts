import type {
    Card,
    CardCounts,
    Experiment,
    Game,
    Item,
    Population,
} from "../types/models";

class Client {
    constructor(public readonly url: string) {}

    getExperiments = async () => {
        let response = await fetch(`${this.url}/experiments`);
        let experiments = (await response.json()) as Experiment[];
        return experiments;
    };

    getPopulations = async (id: string) => {
        let response = await fetch(`${this.url}/experiments/${id}/populations`);
        let populations = (await response.json()) as Population[];
        return populations;
    };

    getItems = async (populationId: string, generation: number) => {
        let response = await fetch(
            `${this.url}/populations/${populationId}/generations/${generation}`
        );
        let items = (await response.json()) as Item[];
        return items;
    };

    getCards = async () => {
        let response = await fetch(`${this.url}/cards`);
        let cards = (await response.json()) as Card[];
        return cards;
    };

    getGames = async (experimentId: string, generation: number) => {
        let response = await fetch(
            `${this.url}/experiments/${experimentId}/generations/${generation}/games`
        );
        let games = (await response.json()) as Game[];
        return games;
    };

    getGame = async (gameId: string) => {
        let response = await fetch(`${this.url}/games/${gameId}`);
        let game = (await response.json()) as Game;
        return game;
    };

    getCardCounts = async (populationId: string) => {
        let response = await fetch(
            `${this.url}/populations/${populationId}/statistics`
        );
        let cardCounts = (await response.json()) as CardCounts[];
        return cardCounts;
    };
}

// TODO: Fix this
export const client = new Client("https://localhost:7079");
