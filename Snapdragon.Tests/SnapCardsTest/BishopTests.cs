namespace Snapdragon.Tests.SnapCardsTest
{
    public class BishopTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void NothingElsePlayed_DoesNotAddPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(3, side, [("Bishop", column)]);

            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AnotherCardPlayed_AddsToPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(3, side, [("Bishop", column)]);
            game = TestHelpers.PlayCards(game, 4, side, [("Ant Man", column)]);
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardPlayedSameTurnButLater_AddsToPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(4, side, [("Bishop", column), ("Ant Man", column)]);
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardPlayedSameTurnButEarlier_DoesNotAddToPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(4, side, [("Ant Man", column), ("Bishop", column)]);
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var bishop = game[column][side][1];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardPlayedOtherSide_DoesNotAddToPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(3, side, [("Bishop", column)]);
            game = TestHelpers.PlayCards(game, 4, side.OtherSide(), [("Ant Man", column)]);
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var bishop = game[column][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void CardPlayedOtherLocation_AddsToPower(Side side)
        {
            var game = TestHelpers.PlayCards(3, side, [("Bishop", Column.Middle)]);
            game = TestHelpers.PlayCards(game, 4, side, [("Ant Man", Column.Left)]);
            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[Column.Middle][side].Count, Is.EqualTo(1));
            var bishop = game[Column.Middle][side][0];

            Assert.That(bishop.Name, Is.EqualTo("Bishop"));

            Assert.That(bishop.Power, Is.EqualTo(2));
        }
    }
}
