using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class ArmorTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void BlocksDestroyingCardsAtSameLocation(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Armor", "Misty Knight");
            game = TestHelpers.PlayCards(game, side.Other(), column, "Elektra");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[column][side][1].Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void DoesNotBlockDestroyingCardsAtOtherLocation(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers.PlayCards(side, column, "Armor");
            game = TestHelpers.PlayCards(game, side, otherColumn, "Misty Knight");
            game = TestHelpers.PlayCards(game, side.Other(), otherColumn, "Elektra");

            Assert.That(game[otherColumn][side].Count, Is.EqualTo(0));

            var destroyedEvent = game
                .PastEvents.OfType<CardDestroyedFromPlayEvent>()
                .SingleOrDefault();
            Assert.That(destroyedEvent, Is.Not.Null);
            Assert.That(destroyedEvent.Card.Side, Is.EqualTo(side));
            Assert.That(destroyedEvent.Card.Name, Is.EqualTo("Misty Knight"));
        }
    }
}
