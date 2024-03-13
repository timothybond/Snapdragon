namespace Snapdragon.Tests.SnapLocationsTest
{
    public class JotunheimTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsUnrevealed))]
        public void DoesNotAffectPowerBeforeReveal(Side side, Column column, int revealTurn)
        {
            var game = TestHelpers
                .NewGame("Jotunheim", column)
                .PlayCards(side, column, "Misty Knight");

            if (game.Turn == revealTurn)
            {
                Assert.Pass(
                    "This test doesn't mean anything, because it's too late to add power to this card before revealing."
                );
            }

            // Really this only matters for Right, where we can end turn 2 also
            while (game.Turn < revealTurn - 1)
            {
                game = game.PlaySingleTurn();
            }

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[column][side][0].Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DecreasesPowerForEachRevealedTurn(Side side, Column column)
        {
            var revealTurn = column.LocationRevealTurn();

            var game = TestHelpers
                .NewGame("Jotunheim", column)
                .PlayCards(side, column, "Misty Knight");

            while (game.Turn < 6)
            {
                game = game.PlaySingleTurn();
            }

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Misty Knight"));

            // I.e., if we reveal on turn 1 we have 6 turn ends after that,
            // and each later turn we reveal subtracts one from that total.
            var turnEndsAfterReveal = 6 - (revealTurn - 1);
            Assert.That(game[column][side][0].Power, Is.EqualTo(2 - turnEndsAfterReveal));
        }
    }
}
