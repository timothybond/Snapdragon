namespace Snapdragon.Tests.SnapCardsTest
{
    public class BlackPantherTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenPlayed_DoublesPower(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Black Panther");

            var blackPanther = game[column][side].Single();

            Assert.That(blackPanther.Power, Is.EqualTo(8)); // 4, doubled
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenModifiedBeforeBeingPlayed_DoublesModifiedPower(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Black Panther")
                .PlayCards(side, column, "Nakia")
                .PlayCards(side, column, "Black Panther");

            var blackPanther = game[column][side].Last();

            Assert.That(blackPanther.Name, Is.EqualTo("Black Panther"));

            Assert.That(blackPanther.Power, Is.EqualTo(10)); // 4 + 1, doubled
        }

        // TODO: Uncomment once Shuri is implemented
        //[Test]
        //[TestCaseSource(typeof(AllSidesAndColumns))]
        //public void WhenModifiedOnPlay_DoublesModifiedPower(Side side, Column column)
        //{
        //    var game = TestHelpers
        //        .NewGame()
        //        .WithCardsInHand(side, "Black Panther")
        //        .PlayCards(side, column, "Shuri")
        //        .PlayCards(side, column, "Black Panther");

        //    var blackPanther = game[column][side].Last();

        //    Assert.That(blackPanther.Name, Is.EqualTo("Black Panther"));

        //    Assert.That(blackPanther.Power, Is.EqualTo(16)); // 4, doubled twice
        //}
    }
}
