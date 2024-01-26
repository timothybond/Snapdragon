using System.Text;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Tests
{
    public class GeneticsTests
    {
        [Test]
        public void PlaysCorrectNumberOfGames()
        {
            var g = new Genetics();

            var decks = g.GetRandomDecks(g.GetInitialCardDefinitions(), 32);

            var engine = new Engine(new NullLogger());

            // Two games per Deck (and two Decks per game) should mean 32 games)
            var wins = g.RunPopulationGames(decks, engine, 2);

            // Technically we will sometimes have draws.
            Assert.That(wins.Sum(), Is.GreaterThan(0));
            Assert.That(wins.Sum(), Is.LessThanOrEqualTo(32));
        }

        [Test]
        public void PopulationTest()
        {
            var g = new Genetics();

            var decks = g.GetRandomDecks(g.GetInitialCardDefinitions(), 32);

            var engine = new Engine(new NullLogger());

            for (var i = 0; i < 100; i++)
            {
                var wins = g.RunPopulationGames(decks, engine, 2);
                decks = g.ReproducePopulation(decks, wins);
            }

            var cardCounts = new Dictionary<string, int>();

            foreach (var deck in decks)
            {
                foreach (var card in deck.Cards)
                {
                    // Card names are ##A, where A (actually A-L) is just to distinguish copies as "unique".
                    // We only care about the numbers, which are Cost and Power.
                    var relevantName = card.Name.Substring(0, 2);

                    if (!cardCounts.ContainsKey(relevantName))
                    {
                        cardCounts[relevantName] = 0;
                    }

                    cardCounts[relevantName] += 1;
                }
            }

            var keys = cardCounts.Keys.OrderBy(k => k).ToList();

            var results = new StringBuilder();
            foreach (var key in keys)
            {
                results.AppendLine($"Card {key}: {cardCounts[key]}");
            }

            Assert.Pass(results.ToString());
        }
    }
}
