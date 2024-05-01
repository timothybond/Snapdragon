namespace Snapdragon.Tests.SnapCardsTest
{
    public class SwarmTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_AddsCopiesToHand(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Swarm");
            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            Assert.That(game[side].Hand[0].Name, Is.EqualTo("Swarm"));
            Assert.That(game[side].Hand[1].Name, Is.EqualTo("Swarm"));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_CopiesHaveNewUniqueIds(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Swarm");

            var oldId = game[side].Hand[0].Id;

            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            Assert.That(game[side].Hand[0].Id, Is.Not.EqualTo(oldId));
            Assert.That(game[side].Hand[1].Id, Is.Not.EqualTo(oldId));
            Assert.That(game[side].Hand[0].Id, Is.Not.EqualTo(game[side].Hand[1].Id));
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_CopiesHaveZeroCost(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Swarm");
            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side].Hand.Count, Is.EqualTo(2));
            Assert.That(game[side].Hand[0].Cost, Is.Zero);
            Assert.That(game[side].Hand[1].Cost, Is.Zero);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscardedTwice_CopiesHaveZeroCost(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Swarm");
            game = game.PlayCards(side, column, "Blade").PlayCards(side, column, "Colleen Wing");

            Assert.That(game[side].Hand.Count, Is.EqualTo(3));
            Assert.That(game[side].Hand[0].Cost, Is.Zero);
            Assert.That(game[side].Hand[1].Cost, Is.Zero);
            Assert.That(game[side].Hand[2].Cost, Is.Zero);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void WhenDiscarded_OpponentHandIsUnchanged(Side side, Column column)
        {
            var game = TestHelpers.NewGame();
            game = game.WithCardsInHand(side, "Swarm");
            game = game.PlayCards(side, column, "Blade");

            Assert.That(game[side.Other()].Hand.Count, Is.EqualTo(0));
        }
    }
}
