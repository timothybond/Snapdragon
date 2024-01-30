namespace Snapdragon.Tests.SnapCardsTest
{
    public class MisterFantasticTests
    {
        [Test]
        [TestCase(Side.Top, Column.Left, 2, 2, 0)]
        [TestCase(Side.Top, Column.Middle, 2, 2, 2)]
        [TestCase(Side.Top, Column.Right, 0, 2, 2)]
        [TestCase(Side.Bottom, Column.Left, 2, 2, 0)]
        [TestCase(Side.Bottom, Column.Middle, 2, 2, 2)]
        [TestCase(Side.Bottom, Column.Right, 0, 2, 2)]
        public void AddsExpectedPower(Side side, Column column, int left, int middle, int right)
        {
            var game = TestHelpers.PlayCards(5, side, [("Mister Fantastic", column)]);

            var scores = game.GetCurrentScores();

            Assert.That(scores[Column.Left][side], Is.EqualTo(left));
            Assert.That(scores[Column.Middle][side], Is.EqualTo(middle));
            Assert.That(scores[Column.Right][side], Is.EqualTo(right));
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Top, Column.Right)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        [TestCase(Side.Bottom, Column.Right)]
        public void DoesNotAddPowerToOtherSide(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(5, side, [("Mister Fantastic", column)]);

            var scores = game.GetCurrentScores();

            Assert.That(scores[Column.Left][side.OtherSide()], Is.EqualTo(0));
            Assert.That(scores[Column.Middle][side.OtherSide()], Is.EqualTo(0));
            Assert.That(scores[Column.Right][side.OtherSide()], Is.EqualTo(0));
        }
    }
}
