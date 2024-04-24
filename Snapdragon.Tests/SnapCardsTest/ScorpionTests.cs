namespace Snapdragon.Tests.SnapCardsTest
{
    public class ScorpionTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void SubtractsFromCardsInOpposingHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Misty Knight", "Blade")
                .PlayCards(side, column, "Scorpion");

            var cardsInHand = game[side.Other()].Hand;
            Assert.That(cardsInHand, Has.Exactly(2).Items);

            Assert.That(cardsInHand[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInHand[0].Power, Is.EqualTo(1));
            Assert.That(cardsInHand[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInHand[1].Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardsStillModifiedOncePlayed(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Misty Knight", "Blade")
                .PlayCards(side, column, "Scorpion")
                .PlayCards(side.Other(), column, "Misty Knight", "Blade");

            var cardsInPlay = game[column][side.Other()];
            Assert.That(cardsInPlay, Has.Exactly(2).Items);

            Assert.That(cardsInPlay[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInPlay[0].Power, Is.EqualTo(1));
            Assert.That(cardsInPlay[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInPlay[1].Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAffectToCardsInOwnHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Misty Knight", "Blade")
                .PlayCards(side, column, "Scorpion");

            var cardsInHand = game[side].Hand;
            Assert.That(cardsInHand, Has.Exactly(2).Items);

            Assert.That(cardsInHand[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInHand[0].Power, Is.EqualTo(2));
            Assert.That(cardsInHand[1].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInHand[1].Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAffectCardsInDeck(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    side.Other(),
                    "Nakia",
                    "Iron Man",
                    "Kraven",
                    "Misty Knight",
                    "Blade"
                )
                .PlayCards(side, column, "Scorpion");

            // Note this will play to turn 2, drawing Nakia and Iron Man but not Kraven, Misty Knight and Blade

            var cardsInDeck = game[side.Other()].Library;
            Assert.That(cardsInDeck, Has.Exactly(3).Items);

            Assert.That(cardsInDeck[0].Name, Is.EqualTo("Kraven"));
            Assert.That(cardsInDeck[0].Power, Is.EqualTo(2));
            Assert.That(cardsInDeck[1].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardsInDeck[1].Power, Is.EqualTo(2));
            Assert.That(cardsInDeck[2].Name, Is.EqualTo("Blade"));
            Assert.That(cardsInDeck[2].Power, Is.EqualTo(3));
        }
    }
}
