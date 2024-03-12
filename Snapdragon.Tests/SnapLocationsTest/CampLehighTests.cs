namespace Snapdragon.Tests.SnapLocationsTest
{
    public class CampLehighTests
    {
        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void AddsThreeCostCardsToHands(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Camp Lehigh", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.StartNextTurn();

            // Note: without further configuration, both players have empty hands/decks otherwise
            Assert.That(game[Side.Top].Hand, Has.Exactly(1).Items);
            Assert.That(game[Side.Top].Hand[0].Cost, Is.EqualTo(3));

            Assert.That(game[Side.Bottom].Hand, Has.Exactly(1).Items);
            Assert.That(game[Side.Bottom].Hand[0].Cost, Is.EqualTo(3));
        }

        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void DoesNotAddCardsBeforeTurn(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Camp Lehigh", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            Assert.That(game[Side.Top].Hand, Is.Empty);
            Assert.That(game[Side.Bottom].Hand, Is.Empty);
        }
    }
}
