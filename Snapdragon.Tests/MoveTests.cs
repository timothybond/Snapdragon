using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    /// <summary>
    /// Tests to validate that we can't normally move stuff.
    /// </summary>
    public class MoveTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void CardWithoutAbility_AttemptingMoveDoesNothing(
            Side side,
            Column initial,
            Column target
        )
        {
            var game = TestHelpers.PlayCards(side, initial, "Misty Knight");

            var playerController = game[side].Controller as TestPlayerController;

            var mistyKnight = game[initial][side].Single();

            var action = new MoveCardAction(side, mistyKnight, initial, target);

            Assert.Throws<InvalidOperationException>(() => action.Apply(game));
        }
    }
}
