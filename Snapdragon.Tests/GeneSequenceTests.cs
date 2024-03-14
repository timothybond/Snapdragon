using System.Collections.Immutable;
using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Tests
{
    public class GeneSequenceTests
    {
        [Test]
        public void Cross_WithFixedCards_DoesNotAllowDuplicates()
        {
            // This test is technically nondeterministic - we can't absolutely guarantee
            // it will fail if there's a bug that produces this outcome. But we will run
            // a bunch of cases so it'll PROBABLY hit it.

            var controller = new MonteCarloSearchController(5);

            // Only 12 cards total, for maximum odds that we'll accidentally screw up (if a bug exists)
            var cardNames = new string[]
            {
                "Ant Man",
                "Hawkeye",
                "Misty Knight",
                "Human Torch",
                "Vulture",
                "Multiple Man",
                "Heimdall",
                "Iron Man",
                "Blade",
                "Agent 13",
                "Hulk",
                "The Thing"
            };

            var allCards = cardNames.Select(name => SnapCards.ByName[name]).ToImmutableList();
            var fixedCards = ImmutableList.Create(
                SnapCards.ByName["Vulture"],
                SnapCards.ByName["Multiple Man"]
            );
            var otherCards = cardNames
                .Except(new[] { "Vulture", "Multiple Man" })
                .Select(name => SnapCards.ByName[name])
                .ToImmutableList();

            Assert.That(otherCards, Has.Exactly(10).Items);

            var genetics = new Genetics(
                fixedCards,
                allCards,
                controller,
                200,
                new RandomCardOrder()
            );

            // Try a bunch of crosses with two parents that are artificially overloaded with the cards in question.
            var alreadyDuplicateParent = new GeneSequence(
                fixedCards,
                [
                    SnapCards.ByName["Vulture"],
                    SnapCards.ByName["Vulture"],
                    SnapCards.ByName["Vulture"],
                    SnapCards.ByName["Vulture"],
                    SnapCards.ByName["Vulture"],
                    SnapCards.ByName["Multiple Man"],
                    SnapCards.ByName["Multiple Man"],
                    SnapCards.ByName["Multiple Man"],
                    SnapCards.ByName["Multiple Man"],
                    SnapCards.ByName["Multiple Man"]
                ],
                genetics,
                controller,
                Guid.NewGuid(),
                null,
                null
            );

            for (var i = 0; i < 100; i++)
            {
                var child = alreadyDuplicateParent.Cross(alreadyDuplicateParent);
                var cards = child.GetCards();

                Assert.That(cards, Has.Exactly(12).Items);

                var distinctCardNames = cards.Select(c => c.Name).Distinct().ToList();
                Assert.That(distinctCardNames, Has.Exactly(12).Items);
            }

            var normalParent = new GeneSequence(
                fixedCards,
                otherCards,
                genetics,
                controller,
                Guid.NewGuid(),
                null,
                null
            );

            for (var i = 0; i < 100; i++)
            {
                var child = normalParent.Cross(normalParent);
                var cards = child.GetCards();

                Assert.That(cards, Has.Exactly(12).Items);

                var distinctCardNames = cards.Select(c => c.Name).Distinct().ToList();
                Assert.That(distinctCardNames, Has.Exactly(12).Items);
            }
        }
    }
}
