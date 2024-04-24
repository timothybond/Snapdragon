namespace Snapdragon.Tests.SnapCardsTest
{
    public class HazmatTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DoesNotModifySelf(Side side, Column column)
        {
            var game = TestHelpers.NewGame().PlayCards(side, column, "Hazmat");

            var hazmat = game[column][side].Single();

            Assert.That(hazmat.Name, Is.EqualTo("Hazmat"));
            Assert.That(hazmat.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ModifiesOtherCardsOnSameSideAndLocation(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Misty Knight", "Nightcrawler", "Hazmat");

            var mistyKnight = game[column][side][0];
            var nightcrawler = game[column][side][1];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(mistyKnight.Power, Is.EqualTo(1));
            Assert.That(nightcrawler.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void ModifiesOtherCardsOnSameSideAndDifferentLocation(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Misty Knight", "Nightcrawler")
                .PlayCards(side, otherColumn, "Hazmat");

            var mistyKnight = game[column][side][0];
            var nightcrawler = game[column][side][1];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(mistyKnight.Power, Is.EqualTo(1));
            Assert.That(nightcrawler.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void ModifiesOtherCardsOnSameLocationAndOtherSide(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Misty Knight", "Nightcrawler")
                .PlayCards(side.Other(), column, "Hazmat");

            var mistyKnight = game[column][side][0];
            var nightcrawler = game[column][side][1];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(mistyKnight.Power, Is.EqualTo(1));
            Assert.That(nightcrawler.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void ModifiesOtherCardsOnDifferentSideAndLocation(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(side, column, "Misty Knight", "Nightcrawler")
                .PlayCards(side.Other(), otherColumn, "Hazmat");

            var mistyKnight = game[column][side][0];
            var nightcrawler = game[column][side][1];

            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));
            Assert.That(nightcrawler.Name, Is.EqualTo("Nightcrawler"));

            Assert.That(mistyKnight.Power, Is.EqualTo(1));
            Assert.That(nightcrawler.Power, Is.EqualTo(1));
        }
    }
}
