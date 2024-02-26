namespace Snapdragon.Tests.SnapCardsTest
{
    public class IronManTests
    {
        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Top, Column.Right)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        [TestCase(Side.Bottom, Column.Right)]
        public async Task Alone_AddsNoPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(5, side, [("Iron Man", column)]);

            var scores = game.GetCurrentScores();

            // Probably overkill
            Assert.That(scores[Column.Left][Side.Top], Is.EqualTo(0));
            Assert.That(scores[Column.Middle][Side.Top], Is.EqualTo(0));
            Assert.That(scores[Column.Right][Side.Top], Is.EqualTo(0));
            Assert.That(scores[Column.Left][Side.Bottom], Is.EqualTo(0));
            Assert.That(scores[Column.Middle][Side.Bottom], Is.EqualTo(0));
            Assert.That(scores[Column.Right][Side.Bottom], Is.EqualTo(0));
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Top, Column.Right)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        [TestCase(Side.Bottom, Column.Right)]
        public async Task WithOtherCards_DoublesPower(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(4, side, [("Hawkeye", column), ("Ant Man", column)]);
            game = await TestHelpers.PlayCards(game, 5, side, [("Iron Man", column)]);
            game = await TestHelpers.PlayCards(game, 6, side, [("Hulk", column)]);

            var scores = game.GetCurrentScores();

            // Ant Man (4) + Hawkeye(4) + Hulk (12) = 20, doubled to 40
            // Ensures that AdjustedPower is included
            Assert.That(scores[column][side], Is.EqualTo(40));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task WithOtherLocationAddingCards_DoublesAllAddedPower(Side side)
        {
            var column = Column.Middle;

            var game = await TestHelpers.PlayCards(3, side, [("Mister Fantastic", Column.Right)]);
            game = await TestHelpers.PlayCards(game, 4, side, [("Hawkeye", column), ("Ant Man", column)]);
            game = await TestHelpers.PlayCards(game, 5, side, [("Iron Man", column)]);
            game = await TestHelpers.PlayCards(game, 6, side, [("Hulk", column)]);

            var scores = game.GetCurrentScores();

            // Same as above case but with another +2 (then doubled) from Mister Fantastic
            Assert.That(scores[column][side], Is.EqualTo(44));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task DoesNotAffectEnemyPower(Side side)
        {
            var column = Column.Middle;

            var game = await TestHelpers.PlayCards(1, side, [("Hawkeye", column)]);
            game = await TestHelpers.PlayCards(game, 2, side, [("Misty Knight", column)]);
            game = await TestHelpers.PlayCards(game, 3, side, [("Mister Fantastic", Column.Right)]);
            game = await TestHelpers.PlayCards(game, 4, side, [("Ant Man", column)]);
            game = await TestHelpers.PlayCards(game, 5, side.Other(), [("Iron Man", column)]);
            game = await TestHelpers.PlayCards(game, 6, side, [("Hulk", column)]);

            var scores = game.GetCurrentScores();

            // Misty Knight (2) + Ant Man (4) + Hawkeye (4) + Hulk (12) + Mister Fantastic effect (2)
            Assert.That(scores[column][side], Is.EqualTo(24));
        }
    }
}
