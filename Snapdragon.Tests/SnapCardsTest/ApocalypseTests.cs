using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class ApocalypseTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnDiscard_ReturnsToHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Apocalypse");

            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));

            // Check that we actually did perform a discard
            var discardEvents = game.PastEvents.OfType<CardDiscardedEvent>().ToList();
            Assert.That(discardEvents.Count, Is.EqualTo(1));
            Assert.That(discardEvents.Single().Card.Name, Is.EqualTo("Apocalypse"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnDiscard_PowerIncreasedByFour(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Apocalypse");
            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));
            Assert.That(game[side].Hand[0].Power, Is.EqualTo(12));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnDiscardTwice_PowerIncreasedByEight(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Apocalypse");
            game = game.PlayCards(side, column, "Blade");
            game = game.PlayCards(side, column, "Lady Sif");

            Assert.That(game[side].Hand.Count, Is.EqualTo(1));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Apocalypse"));
            Assert.That(game[side].Hand[0].Power, Is.EqualTo(16));
        }
    }
}
