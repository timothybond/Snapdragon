using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class ApocalypseTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task OnDiscard_ReturnsToHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Apocalypse");

            game = await game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));

            // Check that we actually did perform a discard
            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>().ToList();
            Assert.That(discardEvents.Count, Is.EqualTo(1));
            Assert.That(discardEvents.Single().Card.Name, Is.EqualTo("Apocalypse"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task OnDiscard_PowerIncreasedByFour(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Apocalypse");
            game = await game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));
            Assert.That(game[side].Hand[0].Power, Is.EqualTo(12));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task OnDiscardTwice_PowerIncreasedByEight(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = await game.WithCardsInHand(side, "Apocalypse")
                .PlayCards(side, column, "Blade")
                .PlayCards(side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));
            Assert.That(game[side].Hand[0].Power, Is.EqualTo(16));
        }
    }
}
