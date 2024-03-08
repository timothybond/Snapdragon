namespace Snapdragon.Tests.SnapCardsTest
{
    public class GreenGoblinTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwitchesSides(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Green Goblin");

            Assert.That(game.AllCards.Count, Is.EqualTo(1));

            Assert.That(game[column][side].Count, Is.EqualTo(0));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            Assert.That(game[column][side.Other()][0].Name, Is.EqualTo("Green Goblin"));
            Assert.That(game[column][side.Other()][0].Power, Is.EqualTo(-3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OtherSideFull_DoesNotSwitchSides(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side.Other(), column, "Ant Man", "Misty Knight")
                .PlayCards(side.Other(), column, "Hawkeye", "Wasp")
                .PlayCards(side, column, "Green Goblin");

            Assert.That(game.AllCards.Count, Is.EqualTo(5));

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(4));

            Assert.That(game[column][side][0].Name, Is.EqualTo("Green Goblin"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(-3));
        }
    }
}
