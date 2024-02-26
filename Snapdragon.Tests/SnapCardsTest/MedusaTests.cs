namespace Snapdragon.Tests.SnapCardsTest
{
    public class MedusaTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task PlayedInMiddle_PowerIsFive(Side side)
        {
            var game = await TestHelpers.PlayCards(2, side, new[] { ("Medusa", Column.Middle) });

            Assert.That(game[Column.Middle][side].Count == 1);

            var medusa = game[Column.Middle][side][0];
            Assert.That(medusa.Name, Is.EqualTo("Medusa"));

            Assert.That(medusa.Power, Is.EqualTo(5));
        }

        [Test]
        [TestCase(Column.Left, Side.Top)]
        [TestCase(Column.Left, Side.Bottom)]
        [TestCase(Column.Right, Side.Top)]
        [TestCase(Column.Right, Side.Bottom)]
        public async Task PlayedNotInMiddle_PowerIsTwo(Column column, Side side)
        {
            var game = await TestHelpers.PlayCards(2, side, new[] { ("Medusa", column) });

            Assert.That(game[column][side].Count == 1);

            var medusa = game[column][side][0];
            Assert.That(medusa.Name, Is.EqualTo("Medusa"));

            Assert.That(medusa.Power, Is.EqualTo(2));
        }
    }
}
