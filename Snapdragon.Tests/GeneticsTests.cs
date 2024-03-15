using System.Text;
using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Tests
{
    public class GeneticsTests
    {
        [Test]
        public async Task PlaysCorrectNumberOfGames()
        {
            var g = new Genetics(
                [],
                TestHelpers.GetInitialCardDefinitions(),
                new RandomPlayerController(),
                100,
                new RandomCardOrder()
            );

            var population = g.GetRandomPopulation(32);

            var engine = new Engine(new NullLogger());

            // Two games per Deck (and two Decks per game) should mean 32 games)
            var wins = await g.RunPopulationGames(population, 2);

            // Technically we will sometimes have draws.
            Assert.That(wins.Sum(), Is.GreaterThan(0));
            Assert.That(wins.Sum(), Is.LessThanOrEqualTo(32));
        }

        [Test]
        public async Task PopulationTestAsync()
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
            var g = new Genetics(
                [],
                TestHelpers.GetInitialCardDefinitions(),
                new RandomPlayerController(),
                100,
                new RandomCardOrder()
            );

            const int DeckCount = 32;
            const int Generations = 100;

            var population = g.GetRandomPopulation(DeckCount);

            var engine = new Engine(new NullLogger());

            for (var i = 0; i < Generations; i++)
            {
                var wins = await g.RunPopulationGames(population, 2);
                population = g.ReproducePopulation(population, wins);
            }

            var cardCounts = new Dictionary<string, int>();

            foreach (var sequence in population)
            {
                foreach (var card in sequence.GetCards())
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

            Assert.That(population.Count, Is.EqualTo(DeckCount));
            Assert.Pass(results.ToString());
        }

        // Disabled due to runtime
        //[Test]
        //[TestCase(10, 256)]
        //[TestCase(20, 256)]
        //[TestCase(40, 256)]
        //[TestCase(80, 256)]
        //[TestCase(160, 256)]
        public async Task PopulationConsistencyTest(int gamesPerDeck, int deckCount)
        {
            // This is a test of how consistently we can expect "better" decks to win.
            // Essentially, we're going to create some random decks and see how well
            // they perform against each other, but then do the same thing again a second time,
            // and see how well their performance correlates each time.
            var genetics = new Genetics(
                [],
                SnapCards.All,
                new MonteCarloSearchController(5),
                200,
                new RandomCardOrder()
            );

            var population = genetics.GetRandomPopulation(deckCount);

            var wins1 = await genetics.RunPopulationGames(population, gamesPerDeck);
            var wins2 = await genetics.RunPopulationGames(population, gamesPerDeck);

            var correlation = MathNet.Numerics.Statistics.Correlation.Pearson(
                wins1.Select(i => (double)i),
                wins2.Select(i => (double)i)
            );

            var results = new StringBuilder();

            results.AppendLine($"Correlation: {correlation}");
            results.AppendLine("Wins:");

            for (var i = 0; i < deckCount; i++)
            {
                results.Append($"{wins1[i]}".PadLeft(3));
                results.Append(", ");
                results.AppendLine($"{wins2[i]}".PadLeft(3));
            }

            Assert.Pass(results.ToString());
        }
    }
}
