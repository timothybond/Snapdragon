namespace Snapdragon.Tests.SnapCardsTest
{
    public class BuckyBarnesTests
    {
        // TODO: Uncomment once Carnage, or some other destructive card, is implemented
        //[Test]
        //[TestCaseSource(typeof(AllSidesAndColumns))]
        //public void WhenDestroyed_ReplacedWithWinterSoldier(Side side, Column column)
        //{
        //    var game = TestHelpers.PlayCards(side, column, "Bucky Barnes").PlayCards(side, column, "Carnage");

        //    var cards = game[column][side];

        //    Assert.That(cards, Has.Exactly(2).Items);
        //    Assert.That(cards[2].Name, Is.EqualTo("Winter Soldier"));
        //    Assert.That(cards[2].Power, Is.EqualTo(6));
        //}

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDestroyedByLocation_ReplacedWithWinterSoldier(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame("Death's Domain", "Death's Domain", "Death's Domain")
                .PlaySingleTurn()
                .PlaySingleTurn() // Need to make sure we reveal the location first
                .PlayCards(side, column, "Bucky Barnes");

            var winterSoldier = game[column][side].Single();

            Assert.That(winterSoldier.Name, Is.EqualTo("Winter Soldier"));
            Assert.That(winterSoldier.Power, Is.EqualTo(6));
        }
    }
}
