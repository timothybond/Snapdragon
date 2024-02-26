namespace Snapdragon.Tests.SnapCardsTest
{
    public class RocketRaccoonTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task PlayCardAlone_PowerIsTwo(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top
                    ? new[] { ("Rocket Raccoon", column) }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Rocket Raccoon", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 1);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Rocket Raccoon"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task PlayCardWithAnotherCardOnSameSide_PowerIsTwo(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                2,
                side == Side.Top
                    ? new[] { ("Rocket Raccoon", column), ("Misty Knight", column) }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Rocket Raccoon", column), ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 2);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Rocket Raccoon"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task PlayCardWithAnotherCardOnOpposingSide_PowerIsFour(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top
                    ? new[] { ("Rocket Raccoon", column) }
                    : new[] { ("Misty Knight", column) },
                side == Side.Bottom
                    ? new[] { ("Rocket Raccoon", column) }
                    : new[] { ("Misty Knight", column) }
            );

            Assert.That(game[column][side].Count == 1);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Rocket Raccoon"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(4));
        }
    }
}
