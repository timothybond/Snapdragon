namespace Snapdragon.Tests.SnapLocationsTest
{
    public class NegativeZoneTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsUnrevealed))]
        public void BeforeReveal_PowerIsNormal(Side side, Column column, int turn)
        {
            var game = TestHelpers
                .NewGame("Negative Zone", column)
                .PlayCards(side, column, "Misty Knight");

            while (game.Turn < turn)
            {
                game = game.PlaySingleTurn();
            }

            var mistyKnight = game[column][side].Single();
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(mistyKnight.AdjustedPower, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesColumnsAndTurnsRevealed))]
        public void AfterReveal_PowerIsAdjusted(Side side, Column column, int turn)
        {
            var game = TestHelpers
                .NewGame("Negative Zone", column)
                .PlayCards(side, column, "Misty Knight");

            while (game.Turn < turn)
            {
                game = game.PlaySingleTurn();
            }

            var mistyKnight = game[column][side].Single();
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(mistyKnight.AdjustedPower, Is.EqualTo(-1));
        }
    }
}
