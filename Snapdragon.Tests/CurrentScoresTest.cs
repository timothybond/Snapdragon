namespace Snapdragon.Tests
{
    public class CurrentScoresTest
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void GetsExpectedTotals_NoAbilities(Side side)
        {
            var game = TestHelpers.PlayCards(1, side, [("Misty Knight", Column.Left)]);
            game = TestHelpers.PlayCards(game, 2, side, [("Wasp", Column.Right)]);
            game = TestHelpers.PlayCards(game, 3, side, [("Cyclops", Column.Middle)]);
            game = TestHelpers.PlayCards(game, 4, side, [("The Thing", Column.Left)]);
            game = TestHelpers.PlayCards(game, 6, side, [("Hulk", Column.Right)]);

            var scores = game.GetCurrentScores();

            Assert.That(scores[Column.Left][side], Is.EqualTo(8)); // Misty Knight (2) + The Thing (6)
            Assert.That(scores[Column.Middle][side], Is.EqualTo(4)); // Cyclops (4)
            Assert.That(scores[Column.Right][side], Is.EqualTo(13)); // Wasp (1) + Hulk (12)
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void GetsExpectedTotals_NotIncludingOtherSide(Side side)
        {
            var game = TestHelpers.PlayCards(1, side, [("Misty Knight", Column.Left)]);
            game = TestHelpers.PlayCards(game, 2, side.OtherSide(), [("Wasp", Column.Right)]);
            game = TestHelpers.PlayCards(game, 3, side, [("Cyclops", Column.Middle)]);
            game = TestHelpers.PlayCards(game, 4, side.OtherSide(), [("The Thing", Column.Left)]);
            game = TestHelpers.PlayCards(game, 6, side, [("Hulk", Column.Right)]);

            var scores = game.GetCurrentScores();

            Assert.That(scores[Column.Left][side], Is.EqualTo(2)); // Misty Knight (2)
            Assert.That(scores[Column.Right][side], Is.EqualTo(12)); // Hulk (12)
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Top, Column.Right)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        [TestCase(Side.Bottom, Column.Right)]
        public void GetsExpectedTotals_UsesAdjustedPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(5, side, [("Hawkeye", column)]);
            game = TestHelpers.PlayCards(
                game,
                6,
                side,
                [("Ant Man", column), ("Misty Knight", column), ("Cyclops", column)]
            );

            var scores = game.GetCurrentScores();

            Assert.That(scores[column][side], Is.EqualTo(14)); // Hawkeye (4) + Ant Man (4) + Misty Knight (2) + Cyclops (4)
        }
    }
}
