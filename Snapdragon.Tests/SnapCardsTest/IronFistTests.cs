using Snapdragon.Events;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class IronFistTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void MovesNextCardRightToMiddle(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Iron Fist")
                .PlayCards(side, Column.Right, "Misty Knight");

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].First().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Middle][side].Last().Name, Is.EqualTo("Misty Knight"));

            Assert.That(
                game[Column.Right][side].Select(c => c.Name).Contains("Misty Knight"),
                Is.False
            );

            var cardMovedEvent = game.PastEvents.OfType<CardMovedEvent>().SingleOrDefault();
            Assert.That(cardMovedEvent, Is.Not.Null);
            Assert.That(cardMovedEvent.Card.Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardMovedEvent.From, Is.EqualTo(Column.Right));
            Assert.That(cardMovedEvent.To, Is.EqualTo(Column.Middle));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void MovesNextCardMiddleToLeft(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Iron Fist")
                .PlayCards(side, Column.Middle, "Misty Knight");

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].First().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Left][side].Last().Name, Is.EqualTo("Misty Knight"));

            Assert.That(
                game[Column.Middle][side].Select(c => c.Name).Contains("Misty Knight"),
                Is.False
            );

            var cardMovedEvent = game.PastEvents.OfType<CardMovedEvent>().SingleOrDefault();
            Assert.That(cardMovedEvent, Is.Not.Null);
            Assert.That(cardMovedEvent.Card.Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardMovedEvent.From, Is.EqualTo(Column.Middle));
            Assert.That(cardMovedEvent.To, Is.EqualTo(Column.Left));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMoveNextCardFromLeft(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Iron Fist")
                .PlayCards(side, Column.Left, "Misty Knight");

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].First().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Left][side].Last().Name, Is.EqualTo("Misty Knight"));

            Assert.That(
                game[Column.Middle][side].Select(c => c.Name).Contains("Misty Knight"),
                Is.False
            );

            var cardMovedEvents = game.PastEvents.OfType<CardMovedEvent>().ToList();
            Assert.That(cardMovedEvents, Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void MovesCardPlayedSameTurn(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                side,
                ("Iron Fist", column),
                ("Misty Knight", Column.Right)
            );

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].First().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Middle][side].Last().Name, Is.EqualTo("Misty Knight"));

            Assert.That(
                game[Column.Right][side].Select(c => c.Name).Contains("Misty Knight"),
                Is.False
            );

            var cardMovedEvent = game.PastEvents.OfType<CardMovedEvent>().SingleOrDefault();
            Assert.That(cardMovedEvent, Is.Not.Null);
            Assert.That(cardMovedEvent.Card.Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardMovedEvent.From, Is.EqualTo(Column.Right));
            Assert.That(cardMovedEvent.To, Is.EqualTo(Column.Middle));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMoveCardRevealedFirst(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                side,
                ("Misty Knight", Column.Right),
                ("Iron Fist", column)
            );

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].Last().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Right][side].First().Name, Is.EqualTo("Misty Knight"));

            var cardMovedEvent = game.PastEvents.OfType<CardMovedEvent>().ToList();
            Assert.That(cardMovedEvent, Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMoveSecondCardPlayed(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Iron Fist")
                .PlayCards(side, Column.Right, "Misty Knight", "Rocket Raccoon");

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side].First().Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Right][side].Last().Name, Is.EqualTo("Rocket Raccoon"));

            var cardMovedEvent = game.PastEvents.OfType<CardMovedEvent>().SingleOrDefault();
            Assert.That(cardMovedEvent, Is.Not.Null);
            Assert.That(cardMovedEvent.Card.Name, Is.EqualTo("Misty Knight"));
            Assert.That(cardMovedEvent.From, Is.EqualTo(Column.Right));
            Assert.That(cardMovedEvent.To, Is.EqualTo(Column.Middle));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotMoveOpponentCard(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, column, "Iron Fist")
                .PlayCards(side.Other(), Column.Right, "Misty Knight");

            // Note: Because of the test cases, we don't know if
            // these end up in the same column at some point
            Assert.That(game[column][side][0].Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Right][side.Other()][0].Name, Is.EqualTo("Misty Knight"));

            var cardMovedEvents = game.PastEvents.OfType<CardMovedEvent>().ToList();
            Assert.That(cardMovedEvents, Is.Empty);
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotMoveSelfOnHulkbusterMerge(Side side)
        {
            var game = TestHelpers
                .PlayCards(side, Column.Right, "Iron Fist")
                .PlayCards(side, Column.Right, "Hulkbuster");

            Assert.That(game[Column.Right][side], Has.Exactly(1).Items);
            Assert.That(game[Column.Right][side][0].Name, Is.EqualTo("Iron Fist"));
            Assert.That(game[Column.Right][side][0].Power, Is.EqualTo(5)); // 2 for Iron Fist, 3 for Hulkbuster
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void MovesPreviouslyPlayedCardOnHulkbusterMerge(Side side)
        {
            var game = TestHelpers
                .PlayCards(side, Column.Right, "Misty Knight")
                .PlayCards(side, Column.Left, "Iron Fist")
                .PlayCards(side, Column.Right, "Hulkbuster");

            Assert.That(game[Column.Right][side], Has.Exactly(0).Items);
            Assert.That(game[Column.Middle][side], Has.Exactly(1).Items);
            Assert.That(game[Column.Middle][side][0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[Column.Middle][side][0].Power, Is.EqualTo(5)); // 2 for Misty Knight, 3 for Hulkbuster
        }
    }
}
