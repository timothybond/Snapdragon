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
            // This is just a basic sanity check of the genetic algorithm.
            //
            // The only assertion is that the population size is correct.
            //
            // It also logs the output population.
            // We should expect cards that are strictly better than one another
            // (in our example population, which is just ability-less cards
            // with a mix of costs and powers) to outcompete inferior ones.
            //
            // Depending on the mutation rate, some inferior cards will still exist.
            //
            // Note also that because this test can take a long time to run if you do
            // a lot of generations, by default it is set to only run a few.
            var g = new Genetics();
            var cards = g.GetInitialCardDefinitions();

            const int DeckCount = 32;
            const int Generations = 10;

            var decks = g.GetRandomDecks(cards, DeckCount);

            var engine = new Engine(new NullLogger());

            for (var i = 0; i < Generations; i++)
            {
                var wins = g.RunPopulationGames(decks, engine, 2);
                decks = g.ReproducePopulation(decks, wins, cards, 1000, c => Random.Next());
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

            Assert.That(decks.Count, Is.EqualTo(DeckCount));
            Assert.Pass(results.ToString());
        }
    }
}
