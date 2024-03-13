using Snapdragon.PlayerActions;

namespace Snapdragon.Tests.SnapLocationsTest
{
    public class KylnTests
    {
        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void CanPlayCardsBeforeTurnFour(Column column, int turn)
        {
            // Note this will always be revealed before turn 4,
            // so even though this is set to be "on the reveal turn",
            // it still conveniently tests turns 1-3.
            var game = TestHelpers.NewGame("Kyln", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(Side.Top, column, "Hawkeye");

            Assert.That(game[column][Side.Top], Has.Exactly(1).Items);
            Assert.That(game[column][Side.Top][0].Name, Is.EqualTo("Hawkeye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CannotPlayCardsOnTurnFive(Side side, Column column)
        {
            var game = TestHelpers.NewGame("Kyln", column);

            for (var i = 0; i < 4; i++)
            {
                game = game.PlaySingleTurn();
            }

            Assert.Throws<InvalidOperationException>(() => game.PlayCards(side, column, "Hawkeye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CannotPlayCardsOnTurnSix(Side side, Column column)
        {
            var game = TestHelpers.NewGame("Kyln", column);

            for (var i = 0; i < 5; i++)
            {
                game = game.PlaySingleTurn();
            }

            Assert.Throws<InvalidOperationException>(() => game.PlayCards(side, column, "Hawkeye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnTurnFive_PlayCardsNotAValidAction(Side side, Column column)
        {
            var game = TestHelpers.NewGame("Kyln", column);

            for (var i = 0; i < 4; i++)
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
