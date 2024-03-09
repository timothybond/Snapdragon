using System.Collections.Immutable;
using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class BladeTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsLastCardInHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new CardInstance(cd, side, CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side] with { Hand = hand });

            game = TestHelpers.PlayCards(game, side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            Assert.That(game[side].Hand.Last().Name, Is.EqualTo(Cards.ThreeThree.Name));

            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);
            Assert.That(discardEvent.Card.Name, Is.EqualTo(Cards.TwoTwo.Name));
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

            game = TestHelpers.PlayCards(game, side, column, "Blade");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
