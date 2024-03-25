namespace Snapdragon.Tests.SnapLocationsTest
{
    public class MurderworldTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardsNotDestroyedBeforeTurnThreeEnds(Side side, Column column)
        {
            // Note: Reveal turn is irrelevant, since it'll always be 3 or earlier
            var game = TestHelpers
                .NewGame("Murderworld", column)
                .PlayCards(side, column, "Misty Knight")
                .PlaySingleTurn()
                .StartNextTurn();

            Assert.That(game[column][side], Has.Exactly(1).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Misty Knight"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardsDestroyedWhenTurnThreeEnds(Side side, Column column)
        {
            // Note: Reveal turn is irrelevant, since it'll always be 3 or earlier
            var game = TestHelpers
                .NewGame("Murderworld", column)
                .PlayCards(side, column, "Misty Knight", "Okoye")
                .PlaySingleTurn()
                .StartNextTurn();

            Assert.That(game[column][side], Has.Exactly(0).Items);

            Assert.That(game[side].Destroyed, Has.Exactly(2).Items);
            Assert.That(game[side].Destroyed[0].Name, Is.EqualTo("Misty Knight"));
            Assert.That(game[side].Destroyed[1].Name, Is.EqualTo("Okoye"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void CardsPlayedAfterTurnThreeNotDestroyed(Side side, Column column)
        {
            // Note: PlayCards will wait until turn 4 to play these because they cost 4 total
            var game = TestHelpers
                .NewGame("Murderworld", column)
                .PlayCards(side, column, "Multiple Man", "Okoye")
                .PlaySingleTurn()
                .PlaySingleTurn();

            Assert.That(game.Turn, Is.EqualTo(6));
            Assert.That(game[column][side], Has.Exactly(2).Items);
            Assert.That(game[column][side][0].Name, Is.EqualTo("Multiple Man"));
            Assert.That(game[column][side][1].Name, Is.EqualTo("Okoye"));
        }
    }
}
