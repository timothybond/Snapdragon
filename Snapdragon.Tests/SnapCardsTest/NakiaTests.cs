namespace Snapdragon.Tests.SnapCardsTest
{
    public class NakiaTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AddsToCardsInHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Nakia", "Misty Knight", "Blade")
                .PlayCards(side, column, "Nakia");

            var cardsInHand = game[side].Hand;
            Assert.That(cardsInHand, Has.Exactly(2).Items);

            Assert.That(cardsInHand[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInHand[0].Power, Is.EqualTo(3));
            Assert.That(cardsInHand[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInHand[1].Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddToCardsInOpponentHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Misty Knight", "Blade")
                .PlayCards(side, column, "Nakia");

            var cardsInHand = game[side.Other()].Hand;
            Assert.That(cardsInHand, Has.Exactly(2).Items);

            Assert.That(cardsInHand[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInHand[0].Power, Is.EqualTo(2));
            Assert.That(cardsInHand[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInHand[1].Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAddToCardsInDeck(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(side, "Nakia", "Iron Man", "Kraven", "Misty Knight", "Blade")
                .PlayCards(side, column, "Nakia");

            // Note this will play to turn 3, drawing Iron Man and Kraven but not Misty Knight and Blade

            var cardsInDeck = game[side].Library.Cards;
            Assert.That(cardsInDeck, Has.Exactly(2).Items);

            Assert.That(cardsInDeck[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInDeck[0].Power, Is.EqualTo(2));
            Assert.That(cardsInDeck[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInDeck[1].Power, Is.EqualTo(3));
        }
    }
}
