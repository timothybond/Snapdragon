using Snapdragon.Events;

namespace Snapdragon.Tests.SnapLocationsTest
{
    public class DeathsDomainTests
    {
        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void DestroysPlayedCard(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Death's Domain", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(Side.Top, column, "Misty Knight");

            Assert.That(game[column][Side.Top], Is.Empty);
            Assert.That(game[Side.Top].Destroyed, Has.Exactly(1).Items);
            Assert.That(game[Side.Top].Destroyed[0].Name, Is.EqualTo("Misty Knight"));

            var destroyEvents = game.PastEvents.OfType<CardDestroyedFromPlayEvent>().ToList();
            Assert.That(destroyEvents, Has.Exactly(1).Items);
            Assert.That(destroyEvents[0].Card.Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void DestroyedCardStillDoesOnReveal(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Death's Domain", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(Side.Top, column, "Squirrel Girl");

            Assert.That(game[column][Side.Top], Is.Empty);

            foreach (var otherColumn in column.Others())
            {
                Assert.That(game[otherColumn][Side.Top], Has.Exactly(1).Items);
                Assert.That(game[otherColumn][Side.Top][0].Name, Is.EqualTo("Squirrel"));
            }
        }

        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void DoesNotDestroyCardBeforeReveal(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Death's Domain", column);

            if (turn == 1)
            {
                Assert.Pass("Pointless test - cannot play before reveal.");
            }

            game = game.PlayCards(Side.Top, column, "Misty Knight");

            Assert.That(game[column][Side.Top], Has.Exactly(1).Items);
            Assert.That(game[column][Side.Top][0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[Side.Top].Destroyed, Is.Empty);
        }
    }
}
