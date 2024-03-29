using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class LadySifTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsHighestCostCardInHand(Side side, Column column)
        {
            // Need to populate some existing cards in the hand
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo);

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));

            var cardsStillInHand = game[side].Hand.Select(c => c.Name).ToList();
            Assert.That(cardsStillInHand, Contains.Item(Cards.OneOne.Name));
            Assert.That(cardsStillInHand, Contains.Item(Cards.TwoTwo.Name));

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo(Cards.ThreeThree.Name));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnlyDiscardsOneCardIfTieForHighestCost(Side side, Column column)
        {
            // Need to populate some existing cards in the hand
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(
                    side,
                    Cards.OneOne,
                    Cards.ThreeThree,
                    Cards.ThreeThree,
                    Cards.TwoTwo
                );

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(3));

            var cardsStillInHand = game[side].Hand.Select(c => c.Name).ToList();
            Assert.That(cardsStillInHand, Contains.Item(Cards.OneOne.Name));
            Assert.That(cardsStillInHand, Contains.Item(Cards.TwoTwo.Name));
            Assert.That(cardsStillInHand, Contains.Item(Cards.ThreeThree.Name));

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo(Cards.ThreeThree.Name));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMakeOpponentDiscard(Side side, Column column)
        {
            // Need to populate some existing cards in the hand
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo);

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
