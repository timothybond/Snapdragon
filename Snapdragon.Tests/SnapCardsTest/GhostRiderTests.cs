namespace Snapdragon.Tests.SnapCardsTest
{
    public class GhostRiderTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void NoDiscards_NothingBreaks(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Ghost Rider");

            Assert.That(game.AllCards, Has.Exactly(1).Items);

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Ghost Rider"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnsOneDiscardToPlay(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Hulk", "Lady Sif", "Blade", "Hawkeye")
                .PlayCards(side, column, "Lady Sif") // Discards Hulk, most expensive card in hand
                .PlayCards(side, column, "Ghost Rider"); // Added to hand to play

            Assert.That(game.AllCards, Has.Exactly(3).Items);

            // All on the same side
            Assert.That(
                game.AllCards.Select(c => c.Side).SequenceEqual(new[] { side, side, side })
            );

            var allCardNames = game.AllCards.Select(c => c.Name);

            Assert.That(allCardNames, Contains.Item("Hulk"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotReturnMultipleDiscards(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Hulk", "Lady Sif", "Blade", "Hawkeye")
                .PlayCards(side, column, "Blade") // Discards Hawkeye, rightmost card in hand
                .PlayCards(side, column, "Lady Sif") // Discards Hulk, most expensive card in hand
                .PlayCards(side, column, "Ghost Rider"); // Added to hand to play

            Assert.That(game.AllCards, Has.Exactly(4).Items);

            // All on the same side
            Assert.That(
                game.AllCards.Select(c => c.Side).SequenceEqual(new[] { side, side, side, side })
            );

            var allCardNames = game.AllCards.Select(c => c.Name);

            Assert.That(allCardNames, Contains.Item("Hulk").Or.Contains("Hawkeye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotAffectOpponentDiscards(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side.Other(), "Hulk", "Lady Sif", "Blade", "Hawkeye")
                .PlayCards(side.Other(), column, "Blade") // Discards Hawkeye, rightmost card in hand
                .PlayCards(side.Other(), column, "Lady Sif") // Discards Hulk, most expensive / only card in hand
                .PlayCards(side, column, "Ghost Rider"); // Added to hand to play

            Assert.That(game.AllCards, Has.Exactly(3).Items);

            // All on the same side
            Assert.That(game.AllCards.Count(c => c.Side == side), Is.EqualTo(1));
            Assert.That(game.AllCards.Count(c => c.Side == side.Other()), Is.EqualTo(2));

            var allCardNames = game.AllCards.Select(c => c.Name);

            Assert.That(allCardNames, Does.Not.Contain("Hulk"));
            Assert.That(allCardNames, Does.Not.Contain("Hawkeye"));
        }
    }
}
