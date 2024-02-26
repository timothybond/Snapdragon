using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class HeimdallTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task MovesOtherCardsLeft(Side side)
        {
            var game = await TestHelpers
                .PlayCards(side, Column.Left, "Ant Man")
                .PlayCards(side, Column.Middle, "Misty Knight")
                .PlayCards(side, Column.Right, "Rocket Raccoon")
                .PlayCards(side, Column.Right, "Heimdall");

            Assert.That(game.AllCards.Count, Is.EqualTo(4));
            Assert.That(game[Column.Left][side][1].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[Column.Middle][side][0].Name, Is.EqualTo("Rocket Raccoon"));
            Assert.That(game[Column.Right][side][0].Name, Is.EqualTo("Heimdall"));

            var moveEvents = game.PastEvents.OfType<CardMovedEvent>().ToList();

            Assert.That(moveEvents.Count, Is.EqualTo(2));

            // Cards are always moved starting from the leftmost ones
            Assert.That(moveEvents[0].Card.Name, Is.EqualTo("Misty Knight"));
            Assert.That(moveEvents[0].From, Is.EqualTo(Column.Middle));
            Assert.That(moveEvents[0].To, Is.EqualTo(Column.Left));

            Assert.That(moveEvents[1].Card.Name, Is.EqualTo("Rocket Raccoon"));
            Assert.That(moveEvents[1].From, Is.EqualTo(Column.Right));
            Assert.That(moveEvents[1].To, Is.EqualTo(Column.Middle));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task DoesNotMoveOpponentCards(Side side)
        {
            var game = await TestHelpers
                .PlayCards(side.Other(), Column.Left, "Ant Man")
                .PlayCards(side.Other(), Column.Middle, "Misty Knight")
                .PlayCards(side.Other(), Column.Right, "Rocket Raccoon")
                .PlayCards(side, Column.Right, "Heimdall");

            Assert.That(game.AllCards.Count, Is.EqualTo(4));
            Assert.That(game[Column.Left][side.Other()][0].Name, Is.EqualTo("Ant Man"));
            Assert.That(game[Column.Middle][side.Other()][0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[Column.Right][side.Other()][0].Name, Is.EqualTo("Rocket Raccoon"));

            var moveEvents = game.PastEvents.OfType<CardMovedEvent>().ToList();

            Assert.That(moveEvents.Count, Is.EqualTo(0));
        }
    }
}
