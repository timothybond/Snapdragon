namespace Snapdragon.Tests.SnapCardsTest
{
    public class CloakTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task OnTurnPlayed_OtherCardsCannotMove(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers.PlayCards(
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
        public async Task OnNextTurn_OtherCardsCanMoveHere(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = await game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public async Task OnNextTurn_OtherCardsCannotMoveToRemainingLocation(
            Side side,
            Column column,
            Column otherColumn,
            Column remainingColumn
        )
        {
            var game = await TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = await game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, remainingColumn), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task OnNextTurn_OpponentCardsCanMoveHere(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = await TestHelpers
                .PlayCards(side.Other(), otherColumn, "Misty Knight")
                .PlayCards(side, column, "Cloak");

            game = await game.StartNextTurn();

            Assert.That(game[otherColumn][side.Other()].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side.Other()][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task OnTwoTurnsLater_OtherCardsCannotMoveHere(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = await TestHelpers.PlayCards(
                side,
                new[] { ("Cloak", column), ("Misty Knight", otherColumn) }
            );

            game = await game.PlaySingleTurn();
            game = await game.StartNextTurn();

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            var mistyKnight = game[otherColumn][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.CanMove(mistyKnight, column), Is.False);
        }
    }
}
