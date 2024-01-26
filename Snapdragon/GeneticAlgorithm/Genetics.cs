using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm;

public class Genetics
{
    /// <summary>
    /// Generates an initial set of cards for use with a basic genetic algorithm.
    ///
    /// In practice this will be 12 each of every combination of the numbers 1-3
    /// for cost and cost-power bonus, where actual power will be that bonus plus the cost.
    /// Some cards are therefore strictly inferior and should be expected to vanish from
    /// the population over time, while others will represent conditional tradeoffs
    /// between less and more expensive cards.
    ///
    /// All cards will be given a three-character name of the form [cost][power]A through [cost][power]L,
    /// since there will be twelve copies of each.
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
    /// Gets a random set of <see cref="Deck"/>s drawn from the specified pool of <see cref="CardDefinition"/>s.
    /// </summary>
    public IReadOnlyList<Deck> GetRandomDecks(
        IReadOnlyList<CardDefinition> cardDefinitions,
        int deckCount
    )
    {
        if (deckCount % 4 != 0)
        {
            throw new ArgumentException($"Deck count must be divisible by 4 (was {deckCount}).");
        }

        var decks = new List<Deck>();

        for (var i = 0; i < deckCount; i++)
        {
            var cards = cardDefinitions.OrderBy(c => Random.Next()).Take(12).ToImmutableList();
            decks.Add(new Deck(cards));
        }

        return decks;
    }

    public List<(int First, int Second)> GetRandomPairs(int populationSize, int pairsPerItem)
    {
        var pairs = new List<(int First, int Second)>();

        var indexesRemaining = new List<int>();
        for (var i = 0; i < populationSize; i++)
        {
            indexesRemaining.Add(i);
        }
        var totalInclusions = indexesRemaining.Select(i => 0).ToList();

        while (indexesRemaining.Count > 1)
        {
            var firstIndex = indexesRemaining[0];
            var secondIndex = indexesRemaining
                .Skip(1 + Random.Next(indexesRemaining.Count - 1))
                .First();

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

    /// <summary>
    /// Runs the specified number of games with each <see cref="Deck"/>,
    /// pairing them randomly against each other.
    /// <returns>The number of wins per each <see cref="Deck"/>.</returns>
    /// </summary>
    public IReadOnlyList<int> RunPopulationGames(
        IReadOnlyList<Deck> decks,
        Engine engine,
        int gamesPerDeck = 5
    )
    {
        var pairs = GetRandomPairs(decks.Count, gamesPerDeck);

        var totalGamesWon = decks.Select(d => 0).ToList();

        foreach (var pair in pairs)
        {
            var topDeckIndex = pair.First;
            var bottomDeckIndex = pair.Second;

            var topPlayerConfig = new PlayerConfiguration(
                $"Deck {topDeckIndex}",
                decks[topDeckIndex],
                new RandomPlayerController()
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                $"Deck {bottomDeckIndex}",
                decks[bottomDeckIndex],
                new RandomPlayerController()
            );

            // TODO: Play game, assign winner, track wins
            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);
            game = engine.PlayGame(game);

            // For the moment we will only count victories.
            var winner = game.GetLeader();
            if (winner != null)
            {
                switch (winner)
                {
                    case Side.Top:
                        totalGamesWon[topDeckIndex] += 1;
                        break;
                    case Side.Bottom:
                        totalGamesWon[bottomDeckIndex] += 1;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        return totalGamesWon;
    }

    public IReadOnlyList<Deck> ReproducePopulation(
        IReadOnlyList<Deck> decks,
        IReadOnlyList<int> wins
    )
    {
        var decksByDescendingWins = decks
            .Select((d, i) => (Deck: d, Wins: wins[i]))
            .OrderByDescending(decksAndWins => decksAndWins.Wins)
            .Select(decksAndWins => decksAndWins.Deck)
            .ToList();

        var survivingDecks = decksByDescendingWins.Take(decksByDescendingWins.Count / 2).ToList();

        var pairs = GetRandomPairs(survivingDecks.Count, 4);

        var nextGeneration = new List<Deck>();

        foreach (var pair in pairs)
        {
            nextGeneration.Add(Cross(survivingDecks[pair.First], survivingDecks[pair.Second]));
        }

        return nextGeneration;
    }

    public Deck Cross(Deck first, Deck second)
    {
        var allCards = first.Cards.Concat(second.Cards).ToList();
        var allCardNames = allCards.Select(c => c.Name).Distinct().ToList();

        // Don't allow duplicates (by name)
        var usedCards = new HashSet<string>();

        var newDeckCards = new List<CardDefinition>();

        for (var i = 0; i < 12; i++)
        {
            var f = first.Cards[i];
            var s = second.Cards[i];

            // The logic when one or more cards is already present
            // probably has some implications for the population.
            // Should look into this later.
            if (!usedCards.Contains(f.Name) && !usedCards.Contains(s.Name))
            {
                var choice = Random.NextBool() ? f : s;

                usedCards.Add(choice.Name);
                newDeckCards.Add(choice);
            }
            else if (!usedCards.Contains(f.Name))
            {
                usedCards.Add(f.Name);
                newDeckCards.Add(f);
            }
            else if (!usedCards.Contains(s.Name))
            {
                usedCards.Add(s.Name);
                newDeckCards.Add(s);
            }
        }

        if (newDeckCards.Count < 12)
        {
            var randomCards = allCardNames
                .Where(n => !usedCards.Contains(n))
                .ToList()
                .OrderBy(n => Random.Next())
                .Take(12 - newDeckCards.Count)
                .Select(n => allCards.First(c => string.Equals(c.Name, n)));

            newDeckCards.AddRange(randomCards);
        }

        return new Deck(newDeckCards.ToImmutableList());
    }
}
