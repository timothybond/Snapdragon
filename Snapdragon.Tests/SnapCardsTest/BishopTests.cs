namespace Snapdragon.Tests.SnapCardsTest
{
    public class BishopTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task NothingElsePlayed_DoesNotAddPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Bishop", column)]);

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task AnotherCardPlayed_AddsToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Bishop", column)]);
            game = await TestHelpers.PlayCards(game, 4, side, [("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedSameTurnButLater_AddsToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(4, side, [("Bishop", column), ("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedSameTurnButEarlier_DoesNotAddToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(4, side, [("Ant Man", column), ("Bishop", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][1];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedOtherSide_DoesNotAddToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Bishop", column)]);
            game = await TestHelpers.PlayCards(game, 4, side.Other(), [("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task CardPlayedOtherLocation_AddsToPower(Side side)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Bishop", Column.Middle)]);
            game = await TestHelpers.PlayCards(game, 4, side, [("Ant Man", Column.Left)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[Column.Middle][side].Count, Is.EqualTo(1));
            var bishop = game[Column.Middle][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }
    }
}
