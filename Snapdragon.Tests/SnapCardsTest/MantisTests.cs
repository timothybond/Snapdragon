namespace Snapdragon.Tests.SnapCardsTest
{
    public class MantisTests
    {
        [Test]
        public void OpponentPlaysCardSameLocation_DrawsOpponentCard()
        {
            // Note that players normally draw three cards to start,
            // but we skipped that by not providing initial decks
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(Side.Top, Cards.OneOne, SnapCards.ByName["Mantis"], Cards.OneThree)
                .WithCardsInDeck(
                    Side.Bottom,
                    SnapCards.ByName["Ant Man"],
                    Cards.OneTwo,
                    Cards.ThreeThree
                )
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Middle) }
                );

            // We drew two cards (one for each turn),
            // then played one, but should have drawn one from the opponent
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(2));
            Assert.That(game[Side.Top].Hand.Last().Definition, Is.EqualTo(Cards.ThreeThree));
        }

        [Test]
        public void OpponentPlaysCardDifferentLocation_DoesNotDraw()
        {
            // Note that players normally draw three cards to start,
            // but we skipped that by not providing initial decks
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    Side.Top,
                    Cards.OneOne,
                    Cards.OneOne,
                    SnapCards.ByName["Mantis"],
                    Cards.OneThree
                )
                .WithCardsInDeck(
                    Side.Bottom,
                    SnapCards.ByName["Ant Man"],
                    Cards.OneOne,
                    Cards.OneOne,
                    Cards.ThreeThree
                )
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Right) }
                );

            // We drew three cards (one per turn), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(2));
        }

        [Test]
        public void PlaysTwoCardsToSameLocation_DoesNotDraw()
        {
            // Note that players normally draw three cards to start,
            // but we skipped that by not providing initial decks
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    Side.Top,
                    SnapCards.ByName["Ant Man"],
                    Cards.OneOne,
                    SnapCards.ByName["Mantis"],
                    Cards.OneThree
                )
                .WithCardsInDeck(Side.Bottom, Cards.OneOne, Cards.OneOne, Cards.ThreeThree)
                .PlayCards(Side.Top, Column.Middle, "Mantis", "Ant Man");

            // We drew two cards (one for each turn), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(1));
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
                    SnapCards.ByName["Mantis"],
                    Cards.OneThree
                )
                .WithCardsInDeck(Side.Bottom, Cards.OneOne, Cards.OneOne, Cards.OneOne)
                .PlayCards(Side.Top, Column.Middle, "Mantis");

            // We drew three cards (one for each turn), then played one
            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(2));
        }
    }
}
