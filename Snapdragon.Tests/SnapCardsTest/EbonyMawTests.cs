﻿using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class EbonyMawTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public void CanPlayOnFirstThreeTurns(Side side, Column first, Column second, Column third)
        {
            var game = TestHelpers
                .PlayCards(side, first, "Ebony Maw")
                .PlayCards(side, second, "Ebony Maw")
                .PlayCards(side, third, "Ebony Maw");

            Assert.That(game.Left[side].Count, Is.EqualTo(1));
            Assert.That(game.Left[side][0].Name, Is.EqualTo("Ebony Maw"));

            Assert.That(game.Middle[side].Count, Is.EqualTo(1));
            Assert.That(game.Middle[side][0].Name, Is.EqualTo("Ebony Maw"));

            Assert.That(game.Right[side].Count, Is.EqualTo(1));
            Assert.That(game.Right[side][0].Name, Is.EqualTo("Ebony Maw"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CannotPlayOnTurnFour(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            while (game.Turn < 3)
            {
                game = game.PlaySingleTurn();
            }

            Assert.Throws<InvalidOperationException>(
                () => game.PlayCards(side, column, "Ebony Maw")
            );
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void NotConsideredPossiblePlayActionOnTurnFour(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInHand(side, "Ebony Maw")
                .StartNextTurn();

            var possibleActionSets = ControllerUtilities.GetPossibleActionSets(game, side);

            // Only result should be "no actions"
            Assert.That(possibleActionSets.Count, Is.EqualTo(1));
            Assert.That(possibleActionSets.Single().Count, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CanPlayOtherCardsBeforeRevealed(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Ebony Maw", "Misty Knight");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[column][side][1].Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void CanPlayOtherCardsElsewhere(Side side, Column first, Column second)
        {
            var game = TestHelpers
                .PlayCards(side, first, "Ebony Maw")
                .PlayCards(side, second, "Misty Knight");

            Assert.That(game[second][side].Count, Is.EqualTo(1));
            Assert.That(game[second][side][0].Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CannotPlayOtherCardsSameLocation(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Ebony Maw");

            Assert.Throws<InvalidOperationException>(
                () => game.PlayCards(side, column, "Misty Knight")
            );
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayOtherCardsSameLocationNotConsideredPossibleMove(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Ebony Maw")
                .WithCardsInHand(side, "Misty Knight")
                .StartNextTurn();

            var possibleActionSets = ControllerUtilities.GetPossibleActionSets(game, side);

            // Play the card in two locations, and also the empty set
            Assert.That(possibleActionSets.Count, Is.EqualTo(3));
            Assert.That(possibleActionSets.Count(s => s.Count == 0), Is.EqualTo(1));

            var nonEmptySets = possibleActionSets.Where(s => s.Count > 0);
            Assert.That(nonEmptySets.All(s => s.Count == 1));

            var possibleActions = nonEmptySets.SelectMany(s => s).OfType<PlayCardAction>().ToList();

            Assert.That(possibleActions.Count, Is.EqualTo(2));
            Assert.That(possibleActions.All(a => string.Equals(a.Card.Name, "Misty Knight")));

            var targetColumns = possibleActions.Select(a => a.Column).ToList();

            foreach (var otherColumn in column.Others())
            {
                Assert.Contains(otherColumn, targetColumns);
            }
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OpponentCanPlayCardsSameLocation(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Ebony Maw")
                .PlayCards(side.Other(), column, "Misty Knight");

            Assert.That(game[column][side.Other()], Has.Exactly(1).Items);
            Assert.That(game[column][side.Other()].Single().Name, Is.EqualTo("Misty Knight"));
        }
    }
}
