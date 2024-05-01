namespace Snapdragon.Tests.SnapCardsTest
{
    public class WongTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoublesOnRevealAtLocationAndSide(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Wong")
                .PlayCards(side, column, "Ironheart");

            var wong = game[column][side].First();

            Assert.That(wong.Name, Is.EqualTo("Wong"));
            Assert.That(wong.Power, Is.EqualTo(6)); // 2 base, +2 from Ironheart twice
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void QuadruplesIfEffectAppearsTwice(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Wong")
                .PlayCards(side, column, "Wong")
                .PlayCards(side, column, "Ironheart");

            var wong = game[column][side].First();

            Assert.That(wong.Name, Is.EqualTo("Wong"));
            Assert.That(wong.Power, Is.EqualTo(10)); // 2 base, +2 from Ironheart four times
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void DoesNotDoubleOnRevealAtOtherLocation(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Wong")
                .PlayCards(side, otherColumn, "White Tiger");

            Assert.That(game.AllCards, Has.Exactly(3).Items);

            var tigerSpirits = game
                .AllCards.Where(c => string.Equals(c.Name, "Tiger Spirit"))
                .ToList();
            Assert.That(tigerSpirits, Has.Exactly(1).Items);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotDoubleOnReveaAtOtherSide(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Wong")
                .PlayCards(side.Other(), column, "White Tiger");

            Assert.That(game.AllCards, Has.Exactly(3).Items);

            var tigerSpirits = game
                .AllCards.Where(c => string.Equals(c.Name, "Tiger Spirit"))
                .ToList();
            Assert.That(tigerSpirits, Has.Exactly(1).Items);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotDoubleWhenUnrevealed(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    side,
                    "Iron Fist",
                    "Misty Knight",
                    "Mister Negative",
                    "Wong",
                    "Ironheart"
                )
                .PlayCards(side, column, "Mister Negative")
                .PlaySingleTurn() // Need to draw modified Ironheart
                .PlayCards(side, column, "Ironheart", "Wong");

            Assert.That(game.AllCards, Has.Exactly(3).Items);
            var misterNegative = game.AllCards.First();

            Assert.That(misterNegative.Name, Is.EqualTo("Mister Negative"));
            Assert.That(misterNegative.Power, Is.EqualTo(1)); // -1 initially, +2 from Ironheart exactly once
        }
    }
}
