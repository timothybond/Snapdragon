namespace Snapdragon.Tests.SnapCardsTest
{
    public class OkoyeTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AddsPowerToAllLibraryCards(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            // (note we skipped the initial draw of 3)
            game = game.WithCardsInDeck(
                side,
                "Agent 13",
                "Elektra",
                "Human Torch",
                "Squirrel Girl"
            );

            game = TestHelpers.PlayCards(game, side, column, "Okoye");

            Assert.That(game[side].Library.Count, Is.EqualTo(2));

            foreach (var cardInLibrary in game[side].Library.Cards)
            {
                Assert.That(cardInLibrary.Power, Is.EqualTo(3));
            }
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddPowerToOpponentLibraryCards(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            // (note we skipped the initial draw of 3)
            game = game.WithCardsInDeck(
                side.Other(),
                "Agent 13",
                "Elektra",
                "Human Torch",
                "Squirrel Girl"
            );

            game = TestHelpers.PlayCards(game, side, column, "Okoye");

            Assert.That(game[side.Other()].Library.Count, Is.EqualTo(2));

            var topCardInLibrary = game[side.Other()].Library[0];

            foreach (var cardInLibrary in game[side.Other()].Library.Cards)
            {
                Assert.That(cardInLibrary.Power, Is.EqualTo(2));
            }
        }
    }
}
