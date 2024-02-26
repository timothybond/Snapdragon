namespace Snapdragon.Tests.SnapCardsTest
{
    public class KravenTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public async Task OnlySelfPlayed_PowerIsTwo(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(side, column, "Kraven");

            game = await game.PlaySingleTurn();
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task CardMovedAway_PowerIsStillTwo(
            Side side,
            Column column,
            Column otherColumn
        )
        {
            var game = await TestHelpers
                .PlayCards(side, column, "Kraven", "Nightcrawler")
                .MoveCards(side, column, otherColumn, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task CardMovedTo_PowerIsFour(Side side, Column column, Column otherColumn)
        {
            var game = await TestHelpers
                .PlayCards(side, otherColumn, "Nightcrawler")
                .PlayCards(side, column, "Kraven")
                .MoveCards(side, otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task EnemyCardMovedTo_PowerIsFour(Side side, Column column, Column otherColumn)
        {
            // TODO: Verify this is actually how Kraven works
            // (the description implies it, but I'm not sure if I've seen it happen)
            var game = await TestHelpers
                .PlayCards(side.Other(), otherColumn, "Nightcrawler")
                .PlayCards(side, column, "Kraven")
                .MoveCards(side.Other(), otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public async Task TwoCardsMovedTo_PowerIsSix(Side side, Column column, Column otherColumn)
        {
            // TODO: Verify this is actually how Kraven works
            // (the description implies it, but I'm not sure if I've seen it happen)
            var game = await TestHelpers
                .PlayCards(side, otherColumn, "Nightcrawler")
                .PlayCards(side.Other(), otherColumn, "Nightcrawler")
                .PlayCards(side, column, "Kraven")
                .MoveCards(side, otherColumn, column, "Nightcrawler")
                .MoveCards(side.Other(), otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(6));
        }
    }
}
