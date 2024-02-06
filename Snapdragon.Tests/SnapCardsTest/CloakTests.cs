namespace Snapdragon.Tests.SnapCardsTest
{
    public class CloakTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void OnTurnPlayed_OtherCardsCannotMove(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void OnNextTurn_OtherCardsCanMoveHere(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public void OnNextTurn_OtherCardsCannotMoveToRemainingLocation(
            Side side,
            Column column,
            Column otherColumn,
            Column remainingColumn
        )
        {
            var game = TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, remainingColumn), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void OnNextTurn_OpponentCardsCanMoveHere(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers
                .PlayCards(side.Other(), otherColumn, "Misty Knight")
                .PlayCards(side, column, "Cloak");

            game = game.StartNextTurn();

            Assert.That(game[otherColumn][side.Other()].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side.Other()][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void OnTwoTurnsLater_OtherCardsCannotMoveHere(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = game.PlaySingleTurn();
            game = game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.False);
        }
    }
}
