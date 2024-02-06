namespace Snapdragon.Tests.SnapCardsTest
{
    public class KravenTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void OnlySelfPlayed_PowerIsTwo(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(side, column, "Kraven");

            game = game.PlaySingleTurn();
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void CardMovedAway_PowerIsStillTwo(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers.PlayCards(side, column, "Kraven", "Nightcrawler");

            game = game.MoveCards(side, column, otherColumn, "Nightcrawler");
            ;

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[otherColumn][side].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void CardMovedTo_PowerIsFour(Side side, Column column, Column otherColumn)
        {
            var game = TestHelpers.PlayCards(side, otherColumn, "Nightcrawler");
            game = game.PlayCards(side, column, "Kraven");

            game = game.MoveCards(side, otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void EnemyCardMovedTo_PowerIsFour(Side side, Column column, Column otherColumn)
        {
            // TODO: Verify this is actually how Kraven works
            // (the description implies it, but I'm not sure if I've seen it happen)
            var game = TestHelpers.PlayCards(side.Other(), otherColumn, "Nightcrawler");
            game = game.PlayCards(side, column, "Kraven");

            game = game.MoveCards(side.Other(), otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(1));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndDifferentColumns))]
        public void TwoCardsMovedTo_PowerIsSix(Side side, Column column, Column otherColumn)
        {
            // TODO: Verify this is actually how Kraven works
            // (the description implies it, but I'm not sure if I've seen it happen)
            var game = TestHelpers.PlayCards(side, otherColumn, "Nightcrawler");
            game = game.PlayCards(side.Other(), otherColumn, "Nightcrawler");
            game = game.PlayCards(side, column, "Kraven");

            game = game.MoveCards(side, otherColumn, column, "Nightcrawler");
            game = game.MoveCards(side.Other(), otherColumn, column, "Nightcrawler");

            Assert.That(game[column][side].Count, Is.EqualTo(2));
            Assert.That(game[column][side.Other()].Count, Is.EqualTo(1));

            var kraven = game[column][side][0];
            Assert.That(kraven.Name, Is.EqualTo("Kraven"));

            Assert.That(kraven.Power, Is.EqualTo(6));
        }
    }
}
