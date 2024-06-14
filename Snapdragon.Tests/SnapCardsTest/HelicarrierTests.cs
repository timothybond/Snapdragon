namespace Snapdragon.Tests.SnapCardsTest
{
    public class HelicarrierTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenPlayed_DoesNothingToHand(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Helicarrier");

            Assert.That(game.AllCards.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand, Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_FillsEmptyHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Helicarrier")
                .PlayCards(side, column, "Lady Sif");

            Assert.That(game.AllCards.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand, Has.Exactly(7).Items);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_FillsHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Helicarrier", "Misty Knight", "Kraven")
                .PlayCards(side, column, "Lady Sif");

            Assert.That(game.AllCards.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand, Has.Exactly(7).Items);
        }
    }
}
