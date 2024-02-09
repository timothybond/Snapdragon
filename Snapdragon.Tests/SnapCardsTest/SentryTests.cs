using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class SentryTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void CannotPlayOnRight(Side side)
        {
            Assert.Throws<InvalidOperationException>(
                () => TestHelpers.PlayCards(side, Column.Right, "Sentry")
            );
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void PlayOnRightNotConsideredPossibleMove(Side side)
        {
            var game = TestHelpers
                .NewGame()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .PlaySingleTurn()
                .WithCardsInHand(side, "Sentry")
                .StartNextTurn();

            var possibleActionSets = ControllerUtilities.GetPossibleActionSets(game, side);

            // Play the card in two locations, and also the empty set
            Assert.That(possibleActionSets.Count, Is.EqualTo(3));
            Assert.That(possibleActionSets.Count(s => s.Count == 0), Is.EqualTo(1));

            var nonEmptySets = possibleActionSets.Where(s => s.Count > 0);
            Assert.That(nonEmptySets.All(s => s.Count == 1));

            var possibleActions = nonEmptySets.SelectMany(s => s).OfType<PlayCardAction>().ToList();

            Assert.That(possibleActions.Count, Is.EqualTo(2));
            Assert.That(possibleActions.All(a => string.Equals(a.Card.Name, "Sentry")));

            var targetColumns = possibleActions.Select(a => a.Column).ToList();

            Assert.Contains(Column.Left, targetColumns);
            Assert.Contains(Column.Middle, targetColumns);
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void AddsVoidToRightSide(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Sentry");

            Assert.That(game.Right[side].Count, Is.EqualTo(1));
            Assert.That(game.Right[side][0].Name, Is.EqualTo("Void"));
            Assert.That(game.Right[side][0].Power, Is.EqualTo(-10));
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void RightSideIsFull_DoesNotAddVoid(Side side, Column column)
        {
            var game = TestHelpers
                .PlayCards(side, Column.Right, "Wasp", "Hawkeye", "Misty Knight", "Ant Man")
                .PlayCards(side, column, "Sentry");

            Assert.That(game.Right[side].Count, Is.EqualTo(4));

            var names = game.Right[side].Select(c => c.Name).ToList();
            Assert.That(names.Contains("Void"), Is.False);
        }
    }
}
