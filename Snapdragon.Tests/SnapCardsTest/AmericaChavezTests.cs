using System.Collections.Immutable;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class AmericaChavezTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AddsPowerToTopLibraryCard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            // (note we skipped the initial draw of 3)
            var cardsInLibrary = Enumerable.Repeat(Cards.OneOne, 4);
            var library = new Library(
                cardsInLibrary.Select(c => new CardInstance(c, side)).ToImmutableList()
            );

            game = game.WithPlayer(game[side] with { Library = library });

            game = TestHelpers.PlayCards(game, side, column, "America Chavez");

            Assert.That(game[side].Library.Count, Is.EqualTo(2));

            var topCardInLibrary = game[side].Library[0];

            Assert.That(topCardInLibrary.Name, Is.EqualTo(Cards.OneOne.Name));
            Assert.That(topCardInLibrary.Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddPowerToNextLibraryCard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            // (note we skipped the initial draw of 3)
            var cardsInLibrary = Enumerable.Repeat(Cards.OneOne, 4);
            var library = new Library(
                cardsInLibrary.Select(c => new CardInstance(c, side)).ToImmutableList()
            );

            game = game.WithPlayer(game[side] with { Library = library });

            game = TestHelpers.PlayCards(game, side, column, "America Chavez");

            Assert.That(game[side].Library.Count, Is.EqualTo(2));

            var nextCardInLibrary = game[side].Library[1];

            Assert.That(nextCardInLibrary.Name, Is.EqualTo(Cards.OneOne.Name));
            Assert.That(nextCardInLibrary.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddPowerToOpponentTopLibraryCard(Side side, Column column)
        {
            var game = TestHelpers.NewGame();

            // Enough sample cards that two will still be in the library
            var cardsInLibrary = Enumerable.Repeat(Cards.OneOne, 4);
            var library = new Library(
                cardsInLibrary.Select(c => new CardInstance(c, side)).ToImmutableList()
            );

            game = game.WithPlayer(game[side.Other()] with { Library = library });

            game = TestHelpers.PlayCards(game, side, column, "America Chavez");

            Assert.That(game[side.Other()].Library.Count, Is.EqualTo(2));

            var topCardInLibrary = game[side.Other()].Library[0];

            Assert.That(topCardInLibrary.Name, Is.EqualTo(Cards.OneOne.Name));
            Assert.That(topCardInLibrary.Power, Is.EqualTo(1));
        }
    }
}
