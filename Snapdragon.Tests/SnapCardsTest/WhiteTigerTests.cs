namespace Snapdragon.Tests.SnapCardsTest
{
    public class WhiteTigerTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AddsTigerToOtherLocation(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "White Tiger");

            var cards = new List<ICard>();
            foreach (var location in game.Locations)
            {
                if (location.Column != column)
                {
                    cards.AddRange(location[side]);
                }
            }

            Assert.That(cards.Count, Is.EqualTo(1));
            Assert.That(cards[0].Name, Is.EqualTo("Tiger Spirit"));
            Assert.That(cards[0].Cost, Is.EqualTo(5));
            Assert.That(cards[0].Power, Is.EqualTo(8));

            // This is just to visually check that the location is randomly chosen
            Assert.Pass(cards[0].Column.ToString());
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddTigerToOwnLocation(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "White Tiger");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side][0].Name, Is.EqualTo("White Tiger"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(1));
        }
    }
}
