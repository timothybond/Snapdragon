namespace Snapdragon.Tests.SnapCardsTest
{
    public class StarLordTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardAlone_PowerIsTwo(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                3,
                side == Side.Top ? new[] { ("Star-Lord", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Star-Lord", column) } : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 1);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Star-Lord"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardWithAnotherCardOnSameSide_PowerIsTwo(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                3,
                side == Side.Top
                    ? new[] { ("Star-Lord", column), ("Misty Knight", column) }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Star-Lord", column), ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 2);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Star-Lord"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardWithAnotherCardOnOpposingSide_PowerIsFive(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                3,
                side == Side.Top
                    ? new[] { ("Star-Lord", column) }
                    : new[] { ("Misty Knight", column) },
                side == Side.Bottom
                    ? new[] { ("Star-Lord", column) }
                    : new[] { ("Misty Knight", column) }
            );

            Assert.That(game[column][side].Count == 1);

            var rocketRaccoon = game[column][side][0];
            Assert.That(rocketRaccoon.Name, Is.EqualTo("Star-Lord"));

            Assert.That(rocketRaccoon.Power, Is.EqualTo(5));
        }
    }
}
