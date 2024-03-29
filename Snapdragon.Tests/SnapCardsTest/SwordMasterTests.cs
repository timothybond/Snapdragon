using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class SwordMasterTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsCardInHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            game = game.WithCardsInHand(side, Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo);
            game = TestHelpers.PlayCards(game, side, column, "Sword Master");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);

            // This is just to visually check that a random card is discarded
            Assert.Pass(discardEvent.Card.Name);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMakeOpponentDiscard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            game = game.WithCardsInHand(side.Other(), Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo);

            game = TestHelpers.PlayCards(game, side, column, "Sword Master");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
