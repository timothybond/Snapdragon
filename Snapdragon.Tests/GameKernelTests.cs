using System.Collections.Immutable;

namespace Snapdragon.Tests
{
    public class GameKernelTests
    {
        private static readonly IReadOnlyList<string> TopCards = new List<string>
        {
            "Ka-Zar",
            "Squirrel Girl",
            "Blue Marvel",
            "Ant Man",
            "Hawkeye",
            "Elektra",
            "Rocket Raccoon",
            "Okoye",
            "Human Torch",
            "Iron Fist",
            "Nightcrawler",
            "Misty Knight"
        };

        private static readonly IReadOnlyList<string> BottomCards = new List<string>
        {
            "Vulture",
            "Human Torch",
            "Doctor Strange",
            "Multiple Man",
            "Kraven",
            "Iron Fist",
            "Cloak",
            "Heimdall",
            "Medusa",
            "Iron Man",
            "Hawkeye",
            "Nightcrawler"
        };

        #region DrawCard

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DrawCard_AddsCardToHand(Side side)
        {
            // See BuildKernel for card order
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);

            Assert.That(hand, Has.Exactly(1).Items);
            Assert.That(kernel.Cards[hand[0]].Name, Is.EqualTo(CardsForSide(side)[0]));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DrawCard_RemovesCardFromDeck(Side side)
        {
            // See BuildKernel for card order
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            var library = GetLibrary(side, kernel);

            Assert.That(library, Has.Exactly(11).Items);
            Assert.That(kernel.Cards[library[0]].Name, Is.EqualTo(CardsForSide(side)[1]));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DrawCard_UpdatesStateToInHand(Side side)
        {
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            Assert.That(kernel.CardStates[hand[0]], Is.EqualTo(CardState.InHand));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DrawCard_ThrowErrorOnEmptyLibrary(Side side)
        {
            var kernel = BuildKernel();

            // Draw the whole deck first
            for (var i = 0; i < 12; i++)
            {
                kernel = kernel.DrawCard(side);
            }

            Assert.Throws<InvalidOperationException>(() => kernel.DrawCard(side));
        }

        #endregion

        #region PlayCard

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayCard_PutsCardInLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel();
            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            var cardsForSide = CardsForSide(side);

            kernel = kernel.PlayCard(hand[0], column, side);

            Assert.That(kernel[column, side], Has.Exactly(1).Items);
            Assert.That(kernel[column, side].First().Name, Is.EqualTo(cardsForSide[0]));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayCard_StoresCardLocation(Side side, Column column)
        {
            var kernel = BuildKernel();
            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            var cardsForSide = CardsForSide(side);

            kernel = kernel.PlayCard(hand[0], column, side);

            Assert.That(kernel.CardLocations[hand[0]], Is.EqualTo(column));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayCard_RemovesCardFromHand(Side side, Column column)
        {
            var kernel = BuildKernel();
            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], column, side);

            hand = GetHand(side, kernel);
            Assert.That(hand, Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayCard_SetsStateToUnrevealed(Side side, Column column)
        {
            var kernel = BuildKernel();
            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], column, side);

            Assert.That(kernel.CardStates[hand[0]], Is.EqualTo(CardState.PlayedButNotRevealed));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayCard_ThrowErrorForCardInLibrary(Side side, Column column)
        {
            var kernel = BuildKernel();

            var library = GetLibrary(side, kernel);
            Assert.Throws<InvalidOperationException>(
                () => kernel.PlayCard(library[0], column, side)
            );
        }

        #endregion

        #region RevealCard

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void RevealCard_SetsStateToInPlay(Side side, Column column)
        {
            var kernel = BuildKernel();
            kernel = kernel.DrawCard(side);

            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], column, side).RevealCard(hand[0]);

            Assert.That(kernel.CardStates[hand[0]], Is.EqualTo(CardState.InPlay));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void RevealCard_ThrowErrorForCardInHand(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);

            var hand = GetHand(side, kernel);
            Assert.Throws<InvalidOperationException>(() => kernel.RevealCard(hand[0]));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void RevealCard_ThrowErrorForCardInLibrary(Side side)
        {
            var kernel = BuildKernel();

            var library = GetLibrary(side, kernel);
            Assert.Throws<InvalidOperationException>(() => kernel.RevealCard(library[0]));
        }

        #endregion

        #region MoveCard

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void MoveCard_ChangesCardLocation(Side side, Column initialColumn, Column newColumn)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], initialColumn, side).RevealCard(hand[0]);

            kernel = kernel.MoveCard(hand[0], side, initialColumn, newColumn);

            Assert.That(kernel.CardLocations[hand[0]], Is.EqualTo(newColumn));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void MoveCard_ChangesUnrevealedCardLocation(
            Side side,
            Column initialColumn,
            Column newColumn
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], initialColumn, side);

            kernel = kernel.MoveCard(hand[0], side, initialColumn, newColumn);

            Assert.That(kernel.CardLocations[hand[0]], Is.EqualTo(newColumn));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndThreeDifferentColumns))]
        public void MoveCard_ThrowsErrorForWrongFrom(
            Side side,
            Column initialColumn,
            Column newColumn,
            Column wrongColumn
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            kernel = kernel.PlayCard(hand[0], initialColumn, side).RevealCard(hand[0]);

            Assert.Throws<InvalidOperationException>(
                () => kernel.MoveCard(hand[0], side, wrongColumn, newColumn)
            );
        }

        #endregion

        #region DiscardCard

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DiscardCard_RemovedFromHand(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);

            kernel = kernel.DiscardCard(hand[0], side);

            hand = GetHand(side, kernel);
            Assert.That(hand, Is.Empty);
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DiscardCard_AddedToDiscards(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);

            kernel = kernel.DiscardCard(hand[0], side);

            var discards = GetDiscards(side, kernel);
            Assert.That(discards, Has.Exactly(1).Items);
            Assert.That(discards[0], Is.EqualTo(hand[0]));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DiscardCard_StateSetToDiscarded(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);

            kernel = kernel.DiscardCard(hand[0], side);

            Assert.That(kernel.CardStates[hand[0]], Is.EqualTo(CardState.Discarded));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DiscardCard_ThrowsErrorForCardInLibrary(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var library = GetLibrary(side, kernel);

            Assert.Throws<InvalidOperationException>(() => kernel.DiscardCard(library[0], side));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DiscardCard_ThrowsErrorForWrongSideInLibrary(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);

            Assert.Throws<InvalidOperationException>(
                () => kernel.DiscardCard(hand[0], side.Other())
            );
        }

        #endregion

        #region DestroyCardFromPlay

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DestroyCardFromPlay_RemovesCardFromLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.DestroyCardFromPlay(cardId, column, side);

            Assert.That(kernel[column, side], Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DestroyCardFromPlay_LeavesPriorColumn(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.DestroyCardFromPlay(cardId, column, side);

            Assert.That(kernel.CardLocations[cardId], Is.EqualTo(column));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DestroyCardFromPlay_SetsStateToDestroyed(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.DestroyCardFromPlay(cardId, column, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.Destroyed));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DestroyCardFromPlay_ThrowsErrorForWrongSide(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            Assert.Throws<InvalidOperationException>(
                () => kernel.DestroyCardFromPlay(cardId, column, side.Other())
            );
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void DestroyCardFromPlay_ThrowsErrorForWrongColumn(
            Side side,
            Column column,
            Column wrongColumn
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            Assert.Throws<InvalidOperationException>(
                () => kernel.DestroyCardFromPlay(cardId, wrongColumn, side)
            );
        }

        #endregion

        #region DestroyCardFromHand

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DestroyCardFromHand_RemovesCardFromHandCollection(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];

            kernel = kernel.DestroyCardFromHand(cardId, side);

            Assert.That(GetHand(side, kernel), Is.Empty);
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DestroyCardFromHand_RemovesCardFromOtherCollections(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];

            kernel = kernel.DestroyCardFromHand(cardId, side);

            Assert.That(kernel.Cards.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardStates.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardLocations.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardSides.ContainsKey(cardId), Is.False);
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DestroyCardFromHand_ThrowsErrorForWrongSide(Side side)
        {
            var kernel = BuildKernel().DrawCard(side);
            var hand = GetHand(side, kernel);
            var cardId = hand[0];

            Assert.Throws<InvalidOperationException>(
                () => kernel.DestroyCardFromHand(cardId, side.Other())
            );
        }

        #endregion

        #region DestroyCardFromLibrary

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DestroyCardFromLibrary_RemovesCardFromLibraryCollection(Side side)
        {
            var kernel = BuildKernel();
            var cardId = GetLibrary(side, kernel)[0];

            kernel = kernel.DestroyCardFromLibrary(cardId, side);

            var library = GetLibrary(side, kernel);
            Assert.That(library, Has.Exactly(11).Items);

            var cardsForSide = CardsForSide(side);
            Assert.That(kernel.Cards[library[0]].Name, Is.EqualTo(cardsForSide[1]));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DestroyCardFromLibrary_RemovesCardFromOtherCollections(Side side)
        {
            var kernel = BuildKernel();
            var cardId = GetLibrary(side, kernel)[0];

            kernel = kernel.DestroyCardFromLibrary(cardId, side);

            Assert.That(kernel.Cards.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardStates.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardLocations.ContainsKey(cardId), Is.False);
            Assert.That(kernel.CardSides.ContainsKey(cardId), Is.False);
        }

        #endregion

        #region ReturnCardToHand

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocation_RemovesFromLocationCollection(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel[column, side], Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocation_AddsCardToHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            var hand = GetHand(side, kernel);
            Assert.That(hand, Has.Exactly(1).Items);
            Assert.That(hand[0], Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocation_SetsStateToInHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InHand));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocationAndRevealed_RemovesFromLocationCollection(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel[column, side], Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocationAndRevealed_AddsCardToHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.ReturnCardToHand(cardId, side);

            var hand = GetHand(side, kernel);
            Assert.That(hand, Has.Exactly(1).Items);
            Assert.That(hand[0], Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_InLocationAndRevealed_SetsStateToInHand(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InHand));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Discarded_RemovesFromDiscardedCollection(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(GetDiscards(side, kernel), Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Discarded_AddsCardToHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            var hand = GetHand(side, kernel);
            Assert.That(hand, Has.Exactly(1).Items);
            Assert.That(hand[0], Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Discarded_SetsStateToInHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InHand));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Destroyed_RemovesFromDestroyedCollection(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(GetDestroyed(side, kernel), Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Destroyed_AddsCardToHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            var hand = GetHand(side, kernel);
            Assert.That(hand, Has.Exactly(1).Items);
            Assert.That(hand[0], Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnCardToHand_Destroyed_SetsStateToInHand(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnCardToHand(cardId, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InHand));
        }

        #endregion

        #region ReturnDiscardToLocation

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDiscardToLocation_AddsToLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnDiscardToLocation(cardId, column, side);

            Assert.That(kernel[column, side], Has.Exactly(1).Items);
            Assert.That(kernel[column, side].First().Id, Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDiscardToLocation_SetsLocation(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnDiscardToLocation(cardId, column, side);

            Assert.That(kernel.CardLocations[cardId], Is.EqualTo(column));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDiscardToLocation_SetsStateToInPlay(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnDiscardToLocation(cardId, column, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InPlay));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDiscardToLocation_RemovesFromDiscardCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel.DiscardCard(cardId, side);

            kernel = kernel.ReturnDiscardToLocation(cardId, column, side);

            Assert.That(GetDiscards(side, kernel), Is.Empty);
        }

        #endregion

        #region ReturnDestroyedToLocation

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDestroyedToLocation_AddsToLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, column, side);

            Assert.That(kernel[column, side], Has.Exactly(1).Items);
            Assert.That(kernel[column, side].First().Id, Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void ReturnDestroyedToLocation_DifferentLocation_AddsToCorrectLocationCollection(
            Side side,
            Column originalColumn,
            Column newColumn
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, originalColumn, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, originalColumn, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, newColumn, side);

            Assert.That(kernel[originalColumn, side], Is.Empty);
            Assert.That(kernel[newColumn, side], Has.Exactly(1).Items);
            Assert.That(kernel[newColumn, side].First().Id, Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDestroyedToLocation_SetsLocation(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, column, side);

            Assert.That(kernel.CardLocations[cardId], Is.EqualTo(column));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void ReturnDestroyedToLocation_DifferentLocation_SetsCorrectLocation(
            Side side,
            Column originalColumn,
            Column newColumn
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, originalColumn, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, originalColumn, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, newColumn, side);

            Assert.That(kernel.CardLocations[cardId], Is.EqualTo(newColumn));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDestroyedToLocation_SetsStateToInPlay(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, column, side);

            Assert.That(kernel.CardStates[cardId], Is.EqualTo(CardState.InPlay));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnDestroyedToLocation_RemovesFromDestroyedCollection(
            Side side,
            Column column
        )
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel)[0];
            kernel = kernel
                .PlayCard(cardId, column, side)
                .RevealCard(cardId)
                .DestroyCardFromPlay(cardId, column, side);

            kernel = kernel.ReturnDestroyedToLocation(cardId, column, side);

            Assert.That(GetDestroyed(side, kernel), Is.Empty);
        }

        #endregion

        #region SwitchSides

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwitchSides_RemovesFromOldLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel).Single();
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.SwitchCardSide(cardId, side);

            Assert.That(kernel[column, side], Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwitchSides_AddsToNewLocationCollection(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel).Single();
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.SwitchCardSide(cardId, side);

            Assert.That(kernel[column, side.Other()], Has.Exactly(1).Items);
            Assert.That(kernel[column, side.Other()].Single().Id, Is.EqualTo(cardId));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwitchSides_UpdatesSide(Side side, Column column)
        {
            var kernel = BuildKernel().DrawCard(side);
            var cardId = GetHand(side, kernel).Single();
            kernel = kernel.PlayCard(cardId, column, side).RevealCard(cardId);

            kernel = kernel.SwitchCardSide(cardId, side);

            Assert.That(kernel.CardSides[cardId], Is.EqualTo(side.Other()));
        }

        #endregion

        #region RevealLocation

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void LocationsStartUnrevealed(Column column)
        {
            var kernel = BuildKernel();

            Assert.That(LocationRevealed(column, kernel), Is.False);
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void SetsLocationRevealedFlag(Column column)
        {
            var kernel = BuildKernel();

            kernel = kernel.RevealLocation(column);

            Assert.That(LocationRevealed(column, kernel), Is.True);
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void DoesNotSetsLocationRevealedFlagForOtherLocations(Column column)
        {
            var kernel = BuildKernel();

            kernel = kernel.RevealLocation(column);

            foreach (var otherColumn in column.Others())
            {
                Assert.That(LocationRevealed(otherColumn, kernel), Is.False);
            }
        }

        #endregion

        #region AddSensorToLocation

        #endregion

        #region DestroySensor

        #endregion

        #region Helper Functions

        public static bool LocationRevealed(Column column, GameKernel kernel)
        {
            switch (column)
            {
                case Column.Left:
                    return kernel.LeftRevealed;
                case Column.Middle:
                    return kernel.MiddleRevealed;
                case Column.Right:
                    return kernel.RightRevealed;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static IReadOnlyList<string> CardsForSide(Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return TopCards;
                case Side.Bottom:
                    return BottomCards;
                default:
                    throw new NotImplementedException();
            }
        }

        private ImmutableList<long> GetDiscards(Side side, GameKernel kernel)
        {
            switch (side)
            {
                case Side.Top:
                    return kernel.TopDiscards;
                case Side.Bottom:
                    return kernel.BottomDiscards;
                default:
                    throw new NotImplementedException();
            }
        }

        private ImmutableList<long> GetDestroyed(Side side, GameKernel kernel)
        {
            switch (side)
            {
                case Side.Top:
                    return kernel.TopDestroyed;
                case Side.Bottom:
                    return kernel.BottomDestroyed;
                default:
                    throw new NotImplementedException();
            }
        }

        private ImmutableList<long> GetHand(Side side, GameKernel kernel)
        {
            switch (side)
            {
                case Side.Top:
                    return kernel.TopHand;
                case Side.Bottom:
                    return kernel.BottomHand;
                default:
                    throw new NotImplementedException();
            }
        }

        private ImmutableList<long> GetLibrary(Side side, GameKernel kernel)
        {
            switch (side)
            {
                case Side.Top:
                    return kernel.TopLibrary;
                case Side.Bottom:
                    return kernel.BottomLibrary;
                default:
                    throw new NotImplementedException();
            }
        }

        private static GameKernel BuildKernel()
        {
            var kaZarDeck = GetDeck(TopCards.ToArray());

            var moveDeck = GetDeck(BottomCards.ToArray());

            var topConfig = new PlayerConfiguration(
                "Ka-Zar",
                kaZarDeck,
                new MonteCarloSearchController(5)
            );
            var bottomConfig = new PlayerConfiguration(
                "Move",
                moveDeck,
                new MonteCarloSearchController(5)
            );

            var topPlayer = new Player(topConfig, Side.Top, 0);
            var bottomPlayer = new Player(bottomConfig, Side.Bottom, 0);

            return GameKernel.FromPlayersAndLocations(
                topPlayer,
                bottomPlayer,
                SnapLocations.ByName["Ruins"],
                SnapLocations.ByName["Ruins"],
                SnapLocations.ByName["Ruins"]
            );
        }

        private static Deck GetDeck(params string[] cardNames)
        {
            if (cardNames.Length != 12)
            {
                throw new ArgumentException("Must specify 12 cards.");
            }

            return new Deck(cardNames.Select(name => SnapCards.ByName[name]).ToImmutableList());
        }

        #endregion
    }
}
