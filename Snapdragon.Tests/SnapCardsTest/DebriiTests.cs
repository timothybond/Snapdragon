namespace Snapdragon.Tests.SnapCardsTest
{
    public class DebriiTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void AddsRockToBothSidesOfOtherLocations(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers.NewGame().PlayCards(side, column, "Debrii");

            Assert.That(game[otherColumn][side], Has.Exactly(1).Items);
            Assert.That(game[otherColumn][side][0].Name, Is.EqualTo("Rock"));
            Assert.That(game[otherColumn][side][0].Power, Is.EqualTo(0));

            Assert.That(game[otherColumn][side.Other()], Has.Exactly(1).Items);
            Assert.That(game[otherColumn][side.Other()][0].Name, Is.EqualTo("Rock"));
            Assert.That(game[otherColumn][side.Other()][0].Power, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddRocksToThisColumn(Side side, Column column)
        {
            var game = TestHelpers.NewGame().PlayCards(side, column, "Debrii");

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Debrii"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(3));

            Assert.That(game[column][side.Other()], Has.Exactly(0).Items);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void DoesNotAddRocksToFullColumns(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, otherColumn, "Ant Man", "Hawkeye", "Misty Knight", "Nightcrawler")
                .PlayCards(
                    side.Other(),
                    otherColumn,
                    "Ant Man",
                    "Hawkeye",
                    "Misty Knight",
                    "Nightcrawler"
                )
                .PlayCards(side, column, "Debrii");

            Assert.That(game[otherColumn][side], Has.Exactly(4).Items);
            Assert.That(game[otherColumn][side].Select(c => c.Name).Contains("Rock"), Is.False);
            Assert.That(game[otherColumn][side.Other()], Has.Exactly(4).Items);
            Assert.That(
                game[otherColumn][side.Other()].Select(c => c.Name).Contains("Rock"),
                Is.False
            );
        }
    }
}
