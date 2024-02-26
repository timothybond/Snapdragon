using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class ElektraTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task EnemyOneCostCard_SameLocation_DestroysCard(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(side.Other(), column, "Ant Man");
            game = await TestHelpers.PlayCards(game, side, column, "Elektra");

            Assert.That(game[column][side.Other()].Count, Is.EqualTo(0));

            var destroyedEvent = game
                .PastEvents.OfType<CardDestroyedFromPlayEvent>()
                .SingleOrDefault();
            Assert.That(destroyedEvent, Is.Not.Null);
            Assert.That(destroyedEvent.Card.Side, Is.EqualTo(side.Other()));
            Assert.That(destroyedEvent.Card.Name, Is.EqualTo("Ant Man"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task EnemyOneCostCard_DifferentLocation_DoesNotDestroyCard(
            Side side,
            Column firstColumn,
            Column secondColumn
        )
        {
            var game = await TestHelpers.PlayCards(side.Other(), firstColumn, "Ant Man");
            game = await TestHelpers.PlayCards(game, side, secondColumn, "Elektra");

            Assert.That(game[firstColumn][side.Other()].Count, Is.EqualTo(1));
            Assert.That(game[firstColumn][side.Other()][0].Name, Is.EqualTo("Ant Man"));

            Assert.That(game.PastEvents.OfType<CardDestroyedFromPlayEvent>(), Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task EnemyTwoCostCard_DoesNotDestroyCard(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(side.Other(), column, "Star-Lord");
            game = await TestHelpers.PlayCards(game, side, column, "Elektra");

            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()][0].Name, Is.EqualTo("Star-Lord"));

            Assert.That(game.PastEvents.OfType<CardDestroyedFromPlayEvent>(), Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task DestroysNewlyPlayedCardIfRevealedFirst(Side side, Column column)
        {
            // Ensures that this side will reveal first
            var game = await TestHelpers.PlayCards(side.Other(), column, "Star-Lord");

            (string, Column)[] topCards =
                side == Side.Top ? [("Elektra", column)] : [("Ant Man", column)];
            (string, Column)[] bottomCards =
                side == Side.Bottom ? [("Elektra", column)] : [("Ant Man", column)];

            game = await TestHelpers.PlayCards(game, 3, topCards, bottomCards);

            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()][0].Name, Is.EqualTo("Star-Lord"));

            var destroyedEvent = game
                .PastEvents.OfType<CardDestroyedFromPlayEvent>()
                .SingleOrDefault();
            Assert.That(destroyedEvent, Is.Not.Null);
            Assert.That(destroyedEvent.Card.Side, Is.EqualTo(side.Other()));
            Assert.That(destroyedEvent.Card.Name, Is.EqualTo("Ant Man"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task DoesNotDestroyPlayedButUnrevealedCard(Side side, Column column)
        {
            // Ensures that this side will reveal first
            var game = await TestHelpers.PlayCards(side, column, "Misty Knight");

            (string, Column)[] topCards =
                side == Side.Top ? [("Elektra", column)] : [("Ant Man", column)];
            (string, Column)[] bottomCards =
                side == Side.Bottom ? [("Elektra", column)] : [("Ant Man", column)];

            game = await TestHelpers.PlayCards(game, 2, topCards, bottomCards);

            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()][0].Name, Is.EqualTo("Ant Man"));

            Assert.That(game.PastEvents.OfType<CardDestroyedFromPlayEvent>(), Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task DoesNotDestroyOwnCard(Side side, Column column)
        {
            // Ensures that this side will reveal first
            var game = await TestHelpers.PlayCards(side, column, "Misty Knight");
            game = await TestHelpers.PlayCards(game, side, column, "Elektra");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[column][side][0].Name, Is.EqualTo("Misty Knight"));

            Assert.That(game.PastEvents.OfType<CardDestroyedFromPlayEvent>(), Is.Empty);
        }
    }
}
