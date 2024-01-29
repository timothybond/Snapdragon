namespace Snapdragon.Tests.SnapCardsTest
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
            game = engine.PlaySingleTurn(game);

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var antMan = game[column][side][0];
            Assert.That(antMan.Name, Is.EqualTo("Ant Man"));

            Assert.That(antMan.Power, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void CardPlayed_PowerIsFour(Side side, Column column)
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

            Assert.That(antMan.Power, Is.EqualTo(4));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void TwoCardsPlayed_PowerIsFour(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                1,
                side == Side.Top ? new[] { ("Ant Man", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Ant Man", column) } : new (string, Column)[] { }
            );

            game = TestHelpers.PlayCards(
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

            var antMan = game[column][side][0];
            Assert.That(antMan.Name, Is.EqualTo("Ant Man"));

            Assert.That(antMan.Power, Is.EqualTo(4));
        }
    }
}
