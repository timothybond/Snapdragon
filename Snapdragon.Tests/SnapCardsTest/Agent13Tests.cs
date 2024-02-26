namespace Snapdragon.Tests.SnapCardsTest
{
    public class Agent13Tests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task AddsCardToHand(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(1, side, [("Agent 13", column)]);

            // Note by default TestHelpers.PlayCards only puts the cards to be played into the hand
            var hand = game[side].Hand;

            Assert.That(hand.Count, Is.EqualTo(1));

            // This is maybe some what pointless
            Assert.That(SnapCards.All.Contains(hand[0].Definition));

            // This is just for a visual check that it's a random card
            Assert.Pass(hand[0].Name);
        }
    }
}
