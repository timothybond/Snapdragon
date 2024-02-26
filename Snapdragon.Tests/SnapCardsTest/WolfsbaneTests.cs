namespace Snapdragon.Tests.SnapCardsTest
{
    public class WolfsbaneTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task PlayedAlone_PowerIsOne(Side side)
        {
            var game = await TestHelpers.PlayCards(3, side, [("Wolfsbane", Column.Middle)]);

            var wolfsbane = game[Column.Middle][side].Single();
            Assert.That(wolfsbane.Name, Is.EqualTo("Wolfsbane"));

            Assert.That(wolfsbane.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, "Hawkeye")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man", "Wasp")]
        [TestCase(Side.Bottom, "Hawkeye")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man", "Wasp")]
        public async Task AddsPowerPerExistingCard(Side side, params string[] otherCardNames)
        {
            var game = await TestHelpers.PlayCards(
                3,
                side,
                otherCardNames.Select(n => (n, Column.Middle))
            );

            game = await TestHelpers.PlayCards(game, 4, side, [("Wolfsbane", Column.Middle)]);

            Assert.That(game[Column.Middle][side].Count, Is.EqualTo(otherCardNames.Length + 1));

            var wolfsbane = game[Column.Middle][side].Last();
            Assert.That(wolfsbane.Name, Is.EqualTo("Wolfsbane"));

            Assert.That(wolfsbane.Power, Is.EqualTo(otherCardNames.Length * 2 + 1));
        }

        [Test]
        [TestCase(Side.Top, "Hawkeye")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man", "Wasp")]
        [TestCase(Side.Bottom, "Hawkeye")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man", "Wasp")]
        public async Task DoesNotAddPowerForEnemyCards(Side side, params string[] otherCardNames)
        {
            var game = await TestHelpers.PlayCards(
                3,
                side.Other(),
                otherCardNames.Select(n => (n, Column.Middle))
            );

            game = await TestHelpers.PlayCards(game, 4, side, [("Wolfsbane", Column.Middle)]);

            var wolfsbane = game[Column.Middle][side].Single();
            Assert.That(wolfsbane.Name, Is.EqualTo("Wolfsbane"));

            Assert.That(wolfsbane.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, "Hawkeye")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man", "Wasp")]
        [TestCase(Side.Bottom, "Hawkeye")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man", "Wasp")]
        public async Task DoesNotAddPowerForCardsAtOtherLocations(
            Side side,
            params string[] otherCardNames
        )
        {
            var game = await TestHelpers.PlayCards(
                3,
                side,
                otherCardNames.Select(n => (n, Column.Right))
            );

            game = await TestHelpers.PlayCards(game, 4, side, [("Wolfsbane", Column.Middle)]);

            var wolfsbane = game[Column.Middle][side].Single();
            Assert.That(wolfsbane.Name, Is.EqualTo("Wolfsbane"));

            Assert.That(wolfsbane.Power, Is.EqualTo(1));
        }
    }
}
