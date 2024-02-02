namespace Snapdragon.Tests.SnapCardsTest
{
    public class SquirrelGirlTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void AddsSquirrelsToOtherColumns(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers.PlayCards(side, column, "Squirrel Girl");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));
            Assert.That(game[otherColumn][side][0].Name, Is.EqualTo("Squirrel"));
            Assert.That(game[otherColumn][side][0].Cost, Is.EqualTo(1));
            Assert.That(game[otherColumn][side][0].Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddAddsSquirrelsToOwnLocation(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Squirrel Girl");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side][0].Name, Is.EqualTo("Squirrel Girl"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddAddsSquirrelsToOtherSide(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Squirrel Girl");

            Assert.That(game[Column.Left][side.Other()].Count, Is.EqualTo(0));
            Assert.That(game[Column.Middle][side.Other()].Count, Is.EqualTo(0));
            Assert.That(game[Column.Right][side.Other()].Count, Is.EqualTo(0));
        }
    }
}
