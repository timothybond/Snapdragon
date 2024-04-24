namespace Snapdragon.Tests.SnapCardsTest
{
    public class LukeCageTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void RemovesScorpionPenaltyOnCardInPlay(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Misty Knight")
                .PlayCards(side.Other(), column, "Scorpion")
                .PlayCards(side, column, "Misty Knight")
                .PlayCards(side, column, "Luke Cage");

            var mistyKnight = game[column][side][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(mistyKnight.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotRemoveScorpionPenaltyOnOpponentCardInPlay(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Misty Knight")
                .PlayCards(side, column, "Scorpion")
                .PlayCards(side.Other(), column, "Misty Knight")
                .PlayCards(side, column, "Luke Cage");

            var mistyKnight = game[column][side.Other()][0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(mistyKnight.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotRemoveScorpionPenaltyOnCardsInHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Misty Knight")
                .PlayCards(side.Other(), column, "Scorpion")
                .PlayCards(side, column, "Luke Cage");

            var mistyKnight = game[side].Hand[0];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(mistyKnight.Power, Is.EqualTo(1));
        }
    }
}
