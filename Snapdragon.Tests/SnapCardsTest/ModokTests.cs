namespace Snapdragon.Tests.SnapCardsTest
{
    public class ModokTests
    {
        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void DiscardsHand(Side side, Column column)
        {
            var game = TestHelpers
                .NewGame()
                .WithCardsInHand(side, "Ant Man", "Hawkeye", "Misty Knight");

            Assert.That(game[side].Hand, Has.Exactly(3).Items);

            game = game.PlayCards(side, column, "Modok");

            Assert.That(game[side].Hand, Is.Empty);
        }

        [Test]
        [TestCaseSource(typeof(AllSidesAndColumns))]
        public void TriggersApocalypseAndSwarmOnceOnly(Side side, Column column)
        {
            var game = TestHelpers.NewGame().WithCardsInHand(side, "Apocalypse", "Swarm");

            Assert.That(game[side].Hand, Has.Exactly(2).Items);

            game = game.PlayCards(side, column, "Modok");

            Assert.That(game[side].Hand, Has.Exactly(3).Items);

            var apocalypse = game[side].Hand.First();
            Assert.That(apocalypse.Name, Is.EqualTo("Apocalypse"));
            Assert.That(apocalypse.Power, Is.EqualTo(10));

            var swarm = game[side].Hand[1];
            Assert.That(swarm.Name, Is.EqualTo("Swarm"));
            Assert.That(swarm.Cost, Is.EqualTo(0));

            swarm = game[side].Hand[2];
            Assert.That(swarm.Name, Is.EqualTo("Swarm"));
            Assert.That(swarm.Cost, Is.EqualTo(0));
        }
    }
}
