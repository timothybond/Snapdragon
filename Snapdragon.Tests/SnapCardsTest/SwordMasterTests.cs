﻿using Snapdragon.Events;
using System.Collections.Immutable;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class SwordMasterTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task DiscardsCardInHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new Card(cd, side, CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side] with { Hand = hand });

            game = await TestHelpers.PlayCards(game, side, column, "Sword Master");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            var discardEvent = game.PastEvents.OfType<CardDiscardedEvent>().SingleOrDefault();

            Assert.That(discardEvent, Is.Not.Null);

            // This is just to visually check that a random card is discarded
            Assert.Pass(discardEvent.Card.Name);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task DoesNotMakeOpponentDiscard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Need to populate some existing cards in the hand
            var hand = new[] { Cards.OneOne, Cards.ThreeThree, Cards.TwoTwo }
                .Select(cd => new Card(cd, side.Other(), CardState.InHand))
                .ToImmutableList();
            game = game.WithPlayer(game[side.Other()] with { Hand = hand });

            game = await TestHelpers.PlayCards(game, side, column, "Sword Master");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(3));

            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>();

            Assert.That(discardEvents.Count(), Is.Zero);
        }
    }
}
