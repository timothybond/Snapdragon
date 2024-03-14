namespace Snapdragon.Tests.SnapCardsTest
{
    public class HelaTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void NoDiscards_NothingBreaks(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Hela");

            Assert.That(game.AllCards, Has.Exactly(1).Items);

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Hela"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ReturnsDiscardsToPlay(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Hulk", "Lady Sif", "Blade", "Hawkeye")
                .PlayCards(side, column, "Blade") // Discards Hawkeye, rightmost card in hand
                .PlayCards(side, column, "Lady Sif") // Discards Hulk, most expensive / only card in hand
                .PlayCards(side, column, "Hela"); // Added to hand to play

            Assert.That(game.AllCards, Has.Exactly(5).Items);

            // All on the same side
            Assert.That(
                game.AllCards.Select(c => c.Side)
                    .SequenceEqual(new[] { side, side, side, side, side })
            );

            // Location can't be over-full (this would only be hit randomly, but might as well check)
            Assert.That(game[column][side].Count, Is.LessThanOrEqualTo(Max.CardsPerLocation));

            var allCardNames = game.AllCards.Select(c => c.Name);

            Assert.That(allCardNames, Contains.Item("Hulk"));
            Assert.That(allCardNames, Contains.Item("Hawkeye"));
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
                .PlayCards(side, column, "Hela"); // Added to hand to play

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
