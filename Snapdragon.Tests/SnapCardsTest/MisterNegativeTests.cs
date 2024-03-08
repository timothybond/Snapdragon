namespace Snapdragon.Tests.SnapCardsTest
{
    public class MisterNegativeTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SwapsCostAndPower(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInDeck(side, "Mister Negative", "Iron Man", "White Tiger")
                .PlayCards(side, column, "Mister Negative");

            var cardsInDeck = game[side].Library.Cards;

            Assert.That(cardsInDeck, Has.Exactly(2).Items);

            Assert.That(cardsInDeck[0].Name, Is.EqualTo("Iron Man"));
            Assert.That(cardsInDeck[0].Cost, Is.EqualTo(0));
            Assert.That(cardsInDeck[0].Power, Is.EqualTo(5));

            Assert.That(cardsInDeck[1].Name, Is.EqualTo("White Tiger"));
            Assert.That(cardsInDeck[1].Cost, Is.EqualTo(1));
            Assert.That(cardsInDeck[1].Power, Is.EqualTo(5));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAffectCardsInHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInHand(side, "Mister Negative", "Iron Man", "White Tiger")
                .PlayCards(side, column, "Mister Negative");

            var cardsInHand = game[side].Hand;

            Assert.That(cardsInHand, Has.Exactly(2).Items);

            Assert.That(cardsInHand[0].Name, Is.EqualTo("Iron Man"));
            Assert.That(cardsInHand[0].Cost, Is.EqualTo(5));
            Assert.That(cardsInHand[0].Power, Is.EqualTo(0));

            Assert.That(cardsInHand[1].Name, Is.EqualTo("White Tiger"));
            Assert.That(cardsInHand[1].Cost, Is.EqualTo(5));
            Assert.That(cardsInHand[1].Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAffectOpponentCards(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInDeck(side.Other(), "Mister Negative", "Iron Man", "White Tiger")
                .PlayCards(side, column, "Mister Negative");

            var cardsInDeck = game[side.Other()].Library.Cards;

            Assert.That(cardsInDeck, Has.Exactly(2).Items);

            Assert.That(cardsInDeck[0].Name, Is.EqualTo("Iron Man"));
            Assert.That(cardsInDeck[0].Cost, Is.EqualTo(5));
            Assert.That(cardsInDeck[0].Power, Is.EqualTo(0));

            Assert.That(cardsInDeck[1].Name, Is.EqualTo("White Tiger"));
            Assert.That(cardsInDeck[1].Cost, Is.EqualTo(5));
            Assert.That(cardsInDeck[1].Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SetsNegativePowerCardsToZeroCost(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInDeck(side, "Mister Negative", "Green Goblin", "Hobgoblin")
                .PlayCards(side, column, "Mister Negative");

            var cardsInDeck = game[side].Library.Cards;

            Assert.That(cardsInDeck, Has.Exactly(2).Items);

            Assert.That(cardsInDeck[0].Name, Is.EqualTo("Green Goblin"));
            Assert.That(cardsInDeck[0].Cost, Is.EqualTo(0));
            Assert.That(cardsInDeck[0].Power, Is.EqualTo(3));

            Assert.That(cardsInDeck[1].Name, Is.EqualTo("Hobgoblin"));
            Assert.That(cardsInDeck[1].Cost, Is.EqualTo(0));
            Assert.That(cardsInDeck[1].Power, Is.EqualTo(5));
        }
    }
}
