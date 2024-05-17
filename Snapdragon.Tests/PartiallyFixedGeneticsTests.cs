using System.Collections.Immutable;
using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Tests
{
    public class PartiallyFixedGeneticsTests
    {
        [Test]
        [TestCase("Ka-Zar")]
        [TestCase("Blue Marvel", "Ka-Zar")]
        public void GeneratesItemsWithExpectedFixedCards(params string[] fixedCardNames)
        {
            var allCards = SnapCards.All.ToImmutableList();

            var fixedCards = fixedCardNames
                .Select(name => SnapCards.ByName[name])
                .ToImmutableList();

            var genetics = new Genetics(
                fixedCards,
                allCards,
                new RandomPlayerController(),
                100,
                new RandomCardOrder()
            );

            for (var i = 0; i < 10; i++)
            {
                var item = genetics.GetRandomItem();

                Assert.That(item.FixedCards.SequenceEqual(fixedCards));
            }
        }

        [Test]
        [TestCase("Ka-Zar")]
        [TestCase("Blue Marvel", "Ka-Zar")]
        public async Task AfterSeveralGenerations_FixedCardsRemainTheSame(
            params string[] fixedCardNames
        )
        {
            const int Generations = 4;

            var allCards = SnapCards.All.ToImmutableList();

            var fixedCards = fixedCardNames
                .Select(name => SnapCards.ByName[name])
                .ToImmutableList();

            var g = new Genetics(
                fixedCards,
                allCards,
                new RandomPlayerController(),
                100,
                new RandomCardOrder()
            );

            var population = g.GetRandomPopulation(8);

            var engine = new Engine(new NullLogger());

            for (var i = 0; i < Generations; i++)
            {
                var wins = await g.RunPopulationGames(population, 2);
                population = g.ReproducePopulation(population, wins);
            }

            Assert.That(population.Count, Is.EqualTo(8));

            foreach (var item in population)
            {
                Assert.That(item.FixedCards.SequenceEqual(fixedCards));
            }
        }

        [Test]
        [TestCase("Apocalypse")]
        [TestCase("Lady Sif", "Sword Master")]
        public void GeneratesItemsWithTwelveCardsTotal(params string[] fixedCardNames)
        {
            var allCards = SnapCards.All.ToImmutableList();

            var fixedCards = fixedCardNames
                .Select(name => SnapCards.ByName[name])
                .ToImmutableList();

            var genetics = new Genetics(
                fixedCards,
                allCards,
                new RandomPlayerController(),
                100,
                new RandomCardOrder()
            );

            for (var i = 0; i < 10; i++)
            {
                var item = genetics.GetRandomItem();

                Assert.That(item.FixedCards.Count + item.EvolvingCards.Count, Is.EqualTo(12));
            }
        }
    }
}
