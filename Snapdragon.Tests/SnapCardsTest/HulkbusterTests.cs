namespace Snapdragon.Tests.SnapCardsTest
{
    public class HulkbusterTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayedAlone_NotMergedOrRemoved(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Hulkbuster");

            var cards = game[column][side];

            Assert.That(cards, Has.Exactly(1).Items);
            Assert.That(cards[0].Name, Is.EqualTo("Hulkbuster"));
            Assert.That(cards[0].Power, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayedWithOnlyEnemyCard_NotMergedOrRemoved(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side.Other(), column, "Hawkeye")
                .PlayCards(side, column, "Hulkbuster");

            var cards = game[column][side];

            Assert.That(cards, Has.Exactly(1).Items);
            Assert.That(cards[0].Name, Is.EqualTo("Hulkbuster"));
            Assert.That(cards[0].Power, Is.EqualTo(3));

            var otherCards = game[column][side.Other()];

            Assert.That(otherCards, Has.Exactly(1).Items);
            Assert.That(otherCards[0].Name, Is.EqualTo("Hawkeye"));
            Assert.That(otherCards[0].Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayedWithSingleCard_Merged(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Misty Knight")
                .PlayCards(side, column, "Hulkbuster");

            var cards = game[column][side];

            Assert.That(cards, Has.Exactly(1).Items);
            Assert.That(cards[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(cards[0].Power, Is.EqualTo(5)); // 2 for Misty Knight base, 3 for Hulkbuster
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void PlayedWithMultipleCards_MergedWithOne(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Wasp", "Misty Knight")
                .PlayCards(side, column, "Hulkbuster");

            var cards = game[column][side];

            Assert.That(cards, Has.Exactly(2).Items);

            var cardNames = cards.Select(c => c.Name).ToList();
            Assert.That(cardNames, Contains.Item("Misty Knight"));
            Assert.That(cardNames, Contains.Item("Wasp"));

            var scores = game.GetCurrentScores();

            var totalScoreForColumn = scores[column][side];
            Assert.That(totalScoreForColumn, Is.EqualTo(6)); // 1 for Wasp, 2 for Misty Knight, 3 from the merged Hulkbuster
        }
    }
}
