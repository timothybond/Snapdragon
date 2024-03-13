using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapLocationsTest
{
    public class SanctumSanctorumTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsUnrevealed))]
        public void BeforeReveal_CanPlayCards(Side side, Column column, int turn)
        {
            var game = TestHelpers.NewGame("Sanctum Sanctorum", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(side, column, "Misty Knight");
            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsRevealed))]
        public void CannotPlayCardsOnAnyTurn(Side side, Column column, int turn)
        {
            var game = TestHelpers.NewGame("Sanctum Sanctorum", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            Assert.Throws<InvalidOperationException>(() => game.PlayCards(side, column, "Hawkeye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsRevealed))]
        public void OnAllTurns_PlayCardsNotAValidAction(Side side, Column column, int turn)
        {
            var game = TestHelpers.NewGame("Sanctum Sanctorum", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.WithCardsInHand(side, "Misty Knight");
            game = game.StartNextTurn();

            var possibleActionSets = ControllerUtilities.GetPossibleActionSets(game, side);

            var possiblePlayCardActions = possibleActionSets
                .SelectMany(s => s)
                .OfType<PlayCardAction>()
                .ToList();

            Assert.That(possiblePlayCardActions.Count, Is.GreaterThan(0));
            Assert.That(
                possiblePlayCardActions.Where(pca => pca.Column == column).Count(),
                Is.EqualTo(0)
            );
        }
    }
}
