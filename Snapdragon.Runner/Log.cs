using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner
{
    internal static class Log
    {
        public static void LogBestDeck(int generation, Population population)
        {
            if (population.Wins == null)
            {
                throw new ArgumentException("'Wins' was unset on the population.");
            }

            var bestDeck = population
                .Items.Select((item, index) => (Item: item, Wins: population.Wins[index]))
                .OrderByDescending(pair => pair.Wins)
                .First();

            var bestCards = bestDeck
                .Item.GetCards()
                .OrderBy(c => c.Cost)
                .ThenBy(c => c.Name)
                .Select(c => c.Name)
                .ToList();

            Console.WriteLine(
                $"Best deck for generation {generation} ({bestDeck.Wins} wins):\n\n{string.Join("\n", bestCards)}\n"
            );
        }

        public static void LogBestDeck(
            int generation,
            IReadOnlyList<GeneSequence> population,
            IReadOnlyList<int> wins
        )
        {
            var bestDeck = population
                .Select((item, index) => (Item: item, Wins: wins[index]))
                .OrderByDescending(pair => pair.Wins)
                .First();

            var bestCards = bestDeck
                .Item.FixedCards.Concat(bestDeck.Item.EvolvingCards)
                .OrderBy(c => c.Cost)
                .ThenBy(c => c.Name)
                .Select(c => c.Name)
                .ToList();

            Console.WriteLine(
                $"Best deck for generation {generation} ({bestDeck.Wins} wins):\n\n{string.Join("\n", bestCards)}\n"
            );
        }

        public static void LogPopulation(
            int generation,
            IReadOnlyList<GeneSequence> population
        )
        {
            var representedCards = population
                .SelectMany(p => p.GetCards().Select(c => c.Name))
                .OrderBy(n => n)
                .Distinct();

            var cardCounts = representedCards.ToDictionary(n => n, n => 0);

            foreach (var item in population)
            {
                foreach (var card in item.GetCards())
                {
                    cardCounts[card.Name] += 1;
                }
            }

            Console.WriteLine($"Generation {generation}:");
            Console.WriteLine();

            foreach (var name in representedCards)
            {
                Console.WriteLine($"{name}: {cardCounts[name]}");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public static List<int> GetCardCounts(IReadOnlyList<GeneSequence> items)
        {
            return SnapCards
                .All.Select(snapCard =>
                    items.Count(i => i.GetCards().Any(c => string.Equals(c.Name, snapCard.Name)))
                )
                .ToList();
        }
    }
}
