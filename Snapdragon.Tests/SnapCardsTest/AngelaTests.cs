namespace Snapdragon.Tests.SnapCardsTest
{
    public class AngelaTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task NothingElsePlayed_DoesNotAddPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(2, side, [("Angela", column)]);

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var angela = game[column][side][0];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task AnotherCardPlayed_AddsToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(2, side, [("Angela", column)]);
            game = await TestHelpers.PlayCards(game, 3, side, [("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var angela = game[column][side][0];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedSameTurnButLater_AddsToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Angela", column), ("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var angela = game[column][side][0];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedSameTurnButEarlier_DoesNotAddToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Ant Man", column), ("Angela", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var angela = game[column][side][1];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task CardPlayedOtherSide_DoesNotAddToPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(2, side, [("Angela", column)]);
            game = await TestHelpers.PlayCards(game, 3, side.Other(), [("Ant Man", column)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var angela = game[column][side][0];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task CardPlayedOtherLocation_DoesNotAddToPowerAsync(Side side)
        {
            var game = await TestHelpers.PlayCards(2, side, [("Angela", Column.Middle)]);
            game = await TestHelpers.PlayCards(game, 3, side, [("Ant Man", Column.Left)]);
            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[Column.Middle][side].Count, Is.EqualTo(1));
            var angela = game[Column.Middle][side][0];

            Assert.That(angela.Name, Is.EqualTo("Angela"));

            Assert.That(angela.Power, Is.EqualTo(2));
        }
    }
}
