using System.Collections.Immutable;
using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class LadySifTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsHighestCostCardInHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new CardInstance(cd, side, CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side] with { Hand = hand });

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));

            var cardsStillInHand = game[side].Hand.Select(c => c.Name).ToList();
            Assert.Contains(Cards.OneOne.Name, cardsStillInHand);
            Assert.Contains(Cards.TwoTwo.Name, cardsStillInHand);

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo(Cards.ThreeThree.Name));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnlyDiscardsOneCardIfTieForHighestCost(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new CardInstance(cd, side, CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side] with { Hand = hand });

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(3));

            var cardsStillInHand = game[side].Hand.Select(c => c.Name).ToList();
            Assert.Contains(Cards.OneOne.Name, cardsStillInHand);
            Assert.Contains(Cards.TwoTwo.Name, cardsStillInHand);
            Assert.Contains(Cards.ThreeThree.Name, cardsStillInHand);

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo(Cards.ThreeThree.Name));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMakeOpponentDiscard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new CardInstance(cd, side.Other(), CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side.Other()] with { Hand = hand });

            game = TestHelpers.PlayCards(game, side, column, "Lady Sif");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
