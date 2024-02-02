using System.Collections.Immutable;

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
            var cardsInLibrary = Enumerable.Repeat(Cards.OneOne, 4);
            var library = new Library(
                cardsInLibrary.Select(c => new Card(c, side)).ToImmutableList()
            );

            game = game.WithPlayer(game[side] with { Library = library });

            game = TestHelpers.PlayCards(game, side, column, "Okoye");

            Assert.That(game[side].Library.Count, Is.EqualTo(2));

            foreach (var cardInLibrary in game[side].Library.Cards)
            {
                Assert.That(cardInLibrary.Name, Is.EqualTo(Cards.OneOne.Name));
                Assert.That(cardInLibrary.Power, Is.EqualTo(2));
            }
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddPowerToOpponentLibraryCards(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            // (note we skipped the initial draw of 3)
            var cardsInLibrary = Enumerable.Repeat(Cards.OneOne, 4);
            var library = new Library(
                cardsInLibrary.Select(c => new Card(c, side)).ToImmutableList()
            );

            game = game.WithPlayer(game[side.Other()] with { Library = library });

            game = TestHelpers.PlayCards(game, side, column, "Okoye");

            Assert.That(game[side.Other()].Library.Count, Is.EqualTo(2));

            var topCardInLibrary = game[side.Other()].Library[0];

            foreach (var cardInLibrary in game[side.Other()].Library.Cards)
            {
                Assert.That(cardInLibrary.Name, Is.EqualTo(Cards.OneOne.Name));
                Assert.That(cardInLibrary.Power, Is.EqualTo(1));
            }
        }
    }
}
