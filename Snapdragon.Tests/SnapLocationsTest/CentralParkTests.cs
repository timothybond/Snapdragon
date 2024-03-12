namespace Snapdragon.Tests.SnapLocationsTest
{
    public class CentralParkTests
    {
        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void AddsSquirrels(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Central Park", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.StartNextTurn();

            foreach (var col in All.Columns)
            {
                foreach (var side in All.Sides)
                {
                    Assert.That(game[col][side], Has.Exactly(1).Items);
                    Assert.That(game[col][side][0].Name, Is.EqualTo("Squirrel"));
                    Assert.That(game[col][side][0].Cost, Is.EqualTo(1));
                    Assert.That(game[col][side][0].Power, Is.EqualTo(1));
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(ColumnsAndRevealTurns))]
        public void DoesNotAddSquirrelsBeforeTurn(Column column, int turn)
        {
            var game = TestHelpers.NewGame("Central Park", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            foreach (var col in All.Columns)
            {
                foreach (var side in All.Sides)
                {
                    Assert.That(game[col][side], Has.Exactly(0).Items);
                }
            }
        }
    }
}
