namespace Snapdragon.Tests.SnapCardsTest
{
    public class CableTests
    {
        [Test]
        public void DrawsOpponentCard()
        {
            // Note that players normally draw three cards to start,
            // but we skipped that by not providing initial decks
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    Side.Top,
                    Cards.OneOne,
                    Cards.OneOne,
                    SnapCards.ByName["Cable"],
                    Cards.OneThree
                )
                .WithCardsInDeck(
                    Side.Bottom,
                    Cards.OneOne,
                    Cards.OneOne,
                    SnapCards.ByName["Cable"],
                    Cards.ThreeThree
                );

            game = game.PlayCards(Side.Top, Column.Middle, "Cable");

            // We drew three cards (one per turn), then played one, but should have drawn one from the opponent.
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(3));
            Assert.That(game[Side.Top].Hand.Last().Definition, Is.EqualTo(Cards.ThreeThree));
        }

        [Test]
        public void OpponentDeckEmpty_NothingBreaks()
        {
            // Note that players normally draw three cards to start,
            // but we skipped that by not providing initial decks
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    Side.Top,
                    Cards.OneOne,
                    Cards.OneOne,
                    SnapCards.ByName["Cable"],
                    Cards.OneThree
                );

            game = game.PlayCards(Side.Top, Column.Middle, "Cable");

            // We drew three cards (one for each turn), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(2));
        }
    }
}
