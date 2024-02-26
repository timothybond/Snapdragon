namespace Snapdragon.Tests.SnapCardsTest
{
    public class HawkeyeTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task NoCardsPlayed_PowerRemainsAtOne(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Hawkeye", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Hawkeye", column) } : new (string, Column)[] { }
            );

            var engine = new Engine(new NullLogger());
            game = await game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var hawkeye = game[column][side][0];
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            Assert.That(hawkeye.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task CardPlayedTooLate_PowerRemainsAtOne(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Hawkeye", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Hawkeye", column) } : new (string, Column)[] { }
            );

            game = await TestHelpers.PlayCards(
                game,
                3,
                side == Side.Top ? new[] { ("Misty Knight", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var hawkeye = game[column][side][0];
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            Assert.That(hawkeye.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task CardPlayed_PowerIsFour(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Hawkeye", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Hawkeye", column) } : new (string, Column)[] { }
            );

            game = await TestHelpers.PlayCards(
                game,
                2,
                side == Side.Top ? new[] { ("Misty Knight", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var hawkeye = game[column][side][0];
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            Assert.That(hawkeye.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public async Task TwoCardsPlayed_PowerIsFour(Side side, Column column)
        {
            var game = await TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Hawkeye", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Hawkeye", column) } : new (string, Column)[] { }
            );

            game = await TestHelpers.PlayCards(
                game,
                2,
                side == Side.Top
                    ? new[] { ("Misty Knight", column), ("Rocket Raccoon", column) }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Misty Knight", column), ("Rocket Raccoon", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(3));

            var hawkeye = game[column][side][0];
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            Assert.That(hawkeye.Power, Is.EqualTo(4));
        }
    }
}
