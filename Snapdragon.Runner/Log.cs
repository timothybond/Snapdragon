using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner
{
    internal static class Log
    {
        public static void LogBestDeck(
            int generation,
            IReadOnlyList<CardGeneSequence> population,
            IReadOnlyList<int> wins
        )
        {
            var bestDeck = population
                .Select((item, index) => (Item: item, Wins: wins[index]))
                .OrderByDescending(pair => pair.Wins)
                .First();

            var bestCards = bestDeck
                .Item.Cards.OrderBy(c => c.Cost)
                .ThenBy(c => c.Name)
                .Select(c => c.Name)
                .ToList();

            Console.WriteLine(
                $"Best deck for generation {generation} ({bestDeck.Wins} wins):\n\n{string.Join("\n", bestCards)}"
            );
        }

        public static void LogPopulation(int generation, IReadOnlyList<CardGeneSequence> population)
        {
            var representedCards = population
                .SelectMany(p => p.Cards.Select(c => c.Name))
                .OrderBy(n => n)
                .Distinct();

            var cardCounts = representedCards.ToDictionary(n => n, n => 0);

            foreach (var item in population)
            {
                foreach (var card in item.Cards)
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

        public static List<int> GetCardCounts(IReadOnlyList<CardGeneSequence> items)
        {
            return SnapCards
                .All.Select(snapCard =>
                    items.Count(i => i.Cards.Any(c => string.Equals(c.Name, snapCard.Name)))
                )
                .ToList();
        }
    }
}
