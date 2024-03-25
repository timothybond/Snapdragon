namespace Snapdragon.Tests.SnapCardsTest
{
    public class JessicaJonesTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void NoCardsPlayed_PowerIsNine(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Jessica Jones").PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var jessicaJones = game[column][side][0];
            Assert.That(jessicaJones.Name, Is.EqualTo("Jessica Jones"));

            Assert.That(jessicaJones.Power, Is.EqualTo(9));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void CardPlayed_PowerRemainsAtFive(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Jessica Jones")
                .PlayCards(side, column, "Misty Knight")
                .PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var jessicaJones = game[column][side][0];
            Assert.That(jessicaJones.Name, Is.EqualTo("Jessica Jones"));

            Assert.That(jessicaJones.Power, Is.EqualTo(5));
        }
    }
}
