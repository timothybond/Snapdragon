namespace Snapdragon.Tests.SnapCardsTest
{
    public class VultureTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenFirstPlayed_PowerIsThree(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Vulture");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var vulture = game[column][side][0];

            Assert.That(vulture.Name, Is.EqualTo("Vulture"));
            Assert.That(vulture.Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void WhenMoved_PowerIsEight(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Vulture")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Vulture");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));
            var vulture = game[otherColumn][side][1];

            Assert.That(vulture.Name, Is.EqualTo("Vulture"));
            Assert.That(vulture.Power, Is.EqualTo(8));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void WhenMovedTwice_PowerIsThirteen(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Vulture")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Vulture")
                .PlayCards(side, column, "Doctor Strange");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            var vulture = game[column][side][1];

            Assert.That(vulture.Name, Is.EqualTo("Vulture"));
            Assert.That(vulture.Power, Is.EqualTo(13));
        }
    }
}
