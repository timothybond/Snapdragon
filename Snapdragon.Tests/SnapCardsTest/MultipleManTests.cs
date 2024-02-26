namespace Snapdragon.Tests.SnapCardsTest
{
    public class MultipleManTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task WhenMoved_CreatesCopy(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers
                .PlayCards(side, column, "Multiple Man")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Multiple Man");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));

            Assert.That(game[column][side][0].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(3));

            Assert.That(game[otherColumn][side][1].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[otherColumn][side][1].Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task WhenMoved_CopyHasDifferentId(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers
                .PlayCards(side, column, "Multiple Man")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Multiple Man");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));

            Assert.That(game[column][side][0].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[otherColumn][side][1].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[column][side][0].Id, Is.Not.EqualTo(game[otherColumn][side][1].Id));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task WhenMoved_CopyIncludesPowerAdjustments(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = await TestHelpers
                .PlayCards(side, column, "Multiple Man")
                .PlayCards(side, column, "Ironheart") // Will now have power 5
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Multiple Man");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));

            Assert.That(game[column][side][1].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[column][side][1].Power, Is.EqualTo(5));

            Assert.That(game[otherColumn][side][1].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[otherColumn][side][1].Power, Is.EqualTo(5));
        }
    }
}
