namespace Snapdragon.Tests.SnapCardsTest
{
    public class OnslaughtTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoublesIronManEffect(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Iron Man")
                .PlayCards(side, column, "Onslaught");

            var scores = game.GetCurrentScores();

            Assert.That(scores[column][side], Is.EqualTo(28)); // Onslaught has 7 power, doubled and then doubled again
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoublesBlueMarvelEffect(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Blue Marvel")
                .PlayCards(side, column, "Onslaught");

            var onslaught = game.AllCards.Skip(1).Single();
            Assert.That(onslaught.Name, Is.EqualTo("Onslaught"));
            Assert.That(onslaught.AdjustedPower, Is.EqualTo(9)); // Onslaught has 7 power, +2 from doubled Blue Marvel
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoublesKlawEffect(Side side)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, Column.Middle, "Klaw")
                .PlayCards(side, Column.Middle, "Onslaught");

            var scores = game.GetCurrentScores();

            Assert.That(scores[Column.Right][side], Is.EqualTo(12)); // Klaw gives +6 power to the location to the right
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoublesWongEffect(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInDeck(
                    side,
                    "Onslaught",
                    "Wong",
                    "Misty Knight",
                    "Mister Negative",
                    "Ironheart"
                )
                .PlayCards(side, column, "Mister Negative")
                .PlayCards(side, column, "Wong")
                .PlayCards(side, column, "Onslaught", "Ironheart");

            var misterNegative = game.AllCards.First();
            Assert.That(misterNegative.Name, Is.EqualTo("Mister Negative"));
            Assert.That(misterNegative.Power, Is.EqualTo(7)); // -1 initially, +2 from Ironheart repeated four times
        }
    }
}
