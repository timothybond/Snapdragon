namespace Snapdragon.Tests.SnapCardsTest
{
    public class HumanTorchTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenFirstPlayed_PowerIsTwo(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Human Torch");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            var humanTorch = game[column][side][0];

            Assert.That(humanTorch.Name, Is.EqualTo("Human Torch"));
            Assert.That(humanTorch.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void WhenMoved_PowerIsFour(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Human Torch")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Human Torch");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));
            var humanTorch = game[otherColumn][side][1];

            Assert.That(humanTorch.Name, Is.EqualTo("Human Torch"));
            Assert.That(humanTorch.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void WhenBuffedThenMoved_BuffIsDoubled(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Human Torch")
                .PlayCards(side, otherColumn, "Cloak")
                .MoveCards(side, column, otherColumn, "Human Torch");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));
            var humanTorch = game[otherColumn][side][1];

            Assert.That(humanTorch.Name, Is.EqualTo("Human Torch"));
            Assert.That(humanTorch.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void WhenMovedTwice_PowerIsEight(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Human Torch")
                .PlayCards(side, column, "Ironheart")
                .PlayCards(side, otherColumn, "Doctor Strange");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(2));
            var humanTorch = game[otherColumn][side][1];

            // Initial power 2, plus 2 from Ironheart, then doubled
            Assert.That(humanTorch.Name, Is.EqualTo("Human Torch"));
            Assert.That(humanTorch.Power, Is.EqualTo(8));
        }
    }
}
