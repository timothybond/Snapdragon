﻿namespace Snapdragon.Tests.SnapCardsTest
{
    public class AntManTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void NoCardsPlayed_PowerRemainsAtOne(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Ant Man", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Ant Man", column) } : new (string, Column)[] { }
            );

            var engine = new Engine(new NullLogger());
            game = game.PlaySingleTurn();

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var antMan = game[column][side][0];
            Assert.That(antMan.Name, Is.EqualTo("Ant Man"));

            Assert.That(antMan.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void CardsPlayedButNotFull_PowerRemainsAtOne(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Ant Man", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Ant Man", column) } : new (string, Column)[] { }
            );

            game = TestHelpers.PlayCards(
                game,
                2,
                side == Side.Top ? new[] { ("Misty Knight", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var antMan = game[column][side][0];
            Assert.That(antMan.Name, Is.EqualTo("Ant Man"));

            Assert.That(antMan.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void FourCardsPlayed_PowerIsFour(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Ant Man", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Ant Man", column) } : new (string, Column)[] { }
            );

            game = TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top
                    ? new[]
                    {
                        ("Misty Knight", column),
                        ("Hawkeye", column),
                        ("Rocket Raccoon", column)
                    }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[]
                    {
                        ("Misty Knight", column),
                        ("Hawkeye", column),
                        ("Rocket Raccoon", column)
                    }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(4));

            var antMan = game[column][side][0];
            Assert.That(antMan.Name, Is.EqualTo("Ant Man"));

            Assert.That(antMan.PowerAdjustment, Is.EqualTo(3));
            Assert.That(antMan.AdjustedPower, Is.EqualTo(4));
        }
    }
}
