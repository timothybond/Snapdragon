using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class BladeTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsLastCardInHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Misty Knight", "Ant Man", "Medusa");

            game = TestHelpers.PlayCards(game, side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            Assert.That(game[side].Hand.Last().Name, Is.EqualTo("Ant Man"));

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo("Medusa"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMakeOpponentDiscard(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Misty Knight", "Ant Man", "Medusa");

            game = TestHelpers.PlayCards(game, side, column, "Blade");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
