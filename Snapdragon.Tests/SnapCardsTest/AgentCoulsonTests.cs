namespace Snapdragon.Tests.SnapCardsTest
{
    public class AgentCoulsonTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void AddsCardsToHand(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(3, side, [("Agent Coulson", column)]);

            // Note by default TestHelpers.PlayCards only puts the cards to be played into the hand
            var hand = game[side].Hand;

            Assert.That(hand.Count, Is.EqualTo(2));

            // This is maybe some what pointless
            Assert.That(SnapCards.All.Contains(hand[0].Definition));
            Assert.That(SnapCards.All.Contains(hand[1].Definition));

            var handByCost = hand.OrderBy(c => c.Cost).ToList();

            Assert.That(handByCost[0].Cost, Is.EqualTo(4));
            Assert.That(handByCost[1].Cost, Is.EqualTo(5));

            // This is just for a visual check that it's two random cards
            Assert.Pass($"{hand[0].Name}\n{hand[1].Name}");
        }
    }
}
