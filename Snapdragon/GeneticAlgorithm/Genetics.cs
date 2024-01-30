namespace Snapdragon.GeneticAlgorithm;

public abstract class Genetics<T>
    where T : IGeneSequence<T>
{
    /// <summary>
    /// Generates an initial set of cards for use with a basic genetic algorithm.  In practice this will be 12 each of
    /// every combination of the numbers 1-3 for cost and cost-power bonus, where actual power will be that bonus plus
    /// the cost. Some cards are therefore strictly inferior and should be expected to vanish from the population over
    /// time, while others will represent conditional tradeoffs between less and more expensive cards.  All cards will
    /// be given a three-character name of the form [cost][power]A through [cost][power]L, since there will be twelve
    /// copies of each.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<CardDefinition> GetInitialCardDefinitions()
    {
        const string labels = "ABCDEFGHIJKL";

        var cards = new List<CardDefinition>();

        for (var cost = 1; cost <= 3; cost++)
        {
            for (var power = 1; power <= 3; power++)
            {
                for (var i = 0; i < 12; i++)
                {
                    var actualPower = cost + power;

                    cards.Add(
                        new CardDefinition($"{cost}{actualPower}{labels[i]}", cost, actualPower)
                    );
                }
            }
        }

        return cards;
    }

    /// <summary>
    /// Gets a random member of the population, based on the overall possible genes.
    /// </summary>
    public IReadOnlyList<T> GetRandomPopulation(int itemCount)
    {
        if (itemCount % 4 != 0)
        {
            throw new ArgumentException($"Item count must be divisible by 4 (was {itemCount}).");
        }

        var population = new List<T>();

        for (var i = 0; i < itemCount; i++)
        {
            population.Add(this.GetRandomItem());
        }

        return population;
    }

    public abstract T GetRandomItem();

    public List<(int First, int Second)> GetRandomPairs(int populationSize, int pairsPerItem)
    {
        var pairs = new List<(int First, int Second)>();

        var indexesRemaining = new List<int>();
        for (var i = 0; i < populationSize; i++)
        {
            indexesRemaining.Add(i);
        }
        var totalInclusions = indexesRemaining.Select(i => 0).ToList();

        while (indexesRemaining.Count > 0)
        {
            var firstIndex = indexesRemaining[0];

            // TODO: Maybe this this? It's possible to have only one item left,
            // so it will only pair with itself at that point.
            var secondIndex =
                indexesRemaining.Count == 1
                    ? indexesRemaining[0]
                    : indexesRemaining.Skip(1 + Random.Next(indexesRemaining.Count - 1)).First();

            pairs.Add((firstIndex, secondIndex));

            foreach (var index in new[] { firstIndex, secondIndex })
            {
                totalInclusions[index] += 1;
                if (totalInclusions[index] >= pairsPerItem)
                {
                    indexesRemaining.Remove(index);
                }
            }
        }

        return pairs;
    }

    public abstract PlayerConfiguration GetPlayerConfiguration(T item, int index);

    /// <summary>
    /// Runs the specified number of games with each <see cref="Deck"/>, pairing them randomly against each other.
    /// </summary>
    /// <returns>The number of wins per each <see cref="Deck"/>.</returns>
    public IReadOnlyList<int> RunPopulationGames(
        IReadOnlyList<T> population,
        Engine engine,
        int gamesPerDeck = 5
    )
    {
        var pairs = GetRandomPairs(population.Count, gamesPerDeck);

        var totalGamesWon = population.Select(d => 0).ToList();

        foreach (var pair in pairs)
        {
            var topIndex = pair.First;
            var bottomIndex = pair.Second;

            var topPlayerConfig = this.GetPlayerConfiguration(population[topIndex], topIndex);
            var bottomPlayerConfig = this.GetPlayerConfiguration(
                population[bottomIndex],
                bottomIndex
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);
            game = game.PlayGame();

            // For the moment we will only count victories, not ties.
            var winner = game.GetLeader();
            if (winner != null)
            {
                switch (winner)
                {
                    case Side.Top:
                        totalGamesWon[topIndex] += 1;
                        break;
                    case Side.Bottom:
                        totalGamesWon[bottomIndex] += 1;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        return totalGamesWon;
    }

    public IReadOnlyList<T> ReproducePopulation(
        IReadOnlyList<T> population,
        IReadOnlyList<int> wins
    )
    {
        var itemsByDescendingWins = population
            .Select((item, index) => (Item: item, Wins: wins[index]))
            .OrderByDescending(itemsAndWins => itemsAndWins.Wins)
            .Select(itemsAndWins => itemsAndWins.Item)
            .ToList();

        var survivingItems = itemsByDescendingWins.Take(itemsByDescendingWins.Count / 2).ToList();

        var pairs = GetRandomPairs(survivingItems.Count, 4);

        var nextGeneration = new List<T>();

        foreach (var pair in pairs)
        {
            nextGeneration.Add(survivingItems[pair.First].Cross(survivingItems[pair.Second]));
        }

        return nextGeneration;
    }
}
