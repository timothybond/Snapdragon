namespace Snapdragon.Tests.SnapLocationsTest
{
    public class MachineworldTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsRevealed))]
        public void CopiesPlayedCard(Side side, Column column, int turn)
        {
            var game = TestHelpers.NewGame("Machineworld", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(side, column, "Wolfsbane");

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Wolfsbane"));

            // Note hands are empty in tests unless otherwise specified
            Assert.That(game[side.Other()].Hand, Has.Exactly(1).Items);
            Assert.That(game[side.Other()].Hand[0].Name, Is.EqualTo("Wolfsbane"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsRevealed))]
        public void CopyHasNewId(Side side, Column column, int turn)
        {
            var game = TestHelpers.NewGame("Machineworld", column);

            for (var i = 1; i < turn; i++)
            {
                game = game.PlaySingleTurn();
            }

            game = game.PlayCards(side, column, "Wolfsbane");

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Wolfsbane"));

            // Note hands are empty in tests unless otherwise specified
            Assert.That(game[side.Other()].Hand, Has.Exactly(1).Items);
            Assert.That(game[side.Other()].Hand[0].Name, Is.EqualTo("Wolfsbane"));

            Assert.That(game[column][side][0].Id, Is.Not.EqualTo(game[side.Other()].Hand[0].Id));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CopyIncludesModifiedTraits(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame("Machineworld", column)
                .PlayCards(side, column, "Wolfsbane")
                .PlayCards(side, column, "Wolfsbane");

            // Note hands are empty in tests unless otherwise specified
            Assert.That(game[side.Other()].Hand, Has.Exactly(2).Items);
            Assert.That(game[side.Other()].Hand[0].Name, Is.EqualTo("Wolfsbane"));
            Assert.That(game[side.Other()].Hand[0].Power, Is.EqualTo(1));
            Assert.That(game[side.Other()].Hand[1].Name, Is.EqualTo("Wolfsbane"));
            Assert.That(game[side.Other()].Hand[1].Power, Is.EqualTo(3));

            // This is more of a sanity check that the power of the other two is as expected
            Assert.That(game[column][side][0].Power, Is.EqualTo(1));
            Assert.That(game[column][side][1].Power, Is.EqualTo(3));
        }
    }
}
