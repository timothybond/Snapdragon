namespace Snapdragon.Tests.SnapCardsTest
{
    public class HobgoblinTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwitchesSides(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Hobgoblin");

            Assert.That(game.AllCards.Count, Is.EqualTo(1));

            Assert.That(game[column][side].Count, Is.EqualTo(0));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            Assert.That(game[column][side.Other()][0].Name, Is.EqualTo("Hobgoblin"));
            Assert.That(game[column][side.Other()][0].Power, Is.EqualTo(-8));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OtherSideFull_DoesNotSwitchSides(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side.Other(), column, "Ant Man", "Misty Knight")
                .PlayCards(side.Other(), column, "Hawkeye", "Wasp")
                .PlayCards(side, column, "Hobgoblin");

            Assert.That(game.AllCards.Count, Is.EqualTo(5));

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(4));

            Assert.That(game[column][side][0].Name, Is.EqualTo("Hobgoblin"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(-8));
        }
    }
}
