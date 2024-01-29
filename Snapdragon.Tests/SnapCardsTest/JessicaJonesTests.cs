namespace Snapdragon.Tests.SnapCardsTest
{
    public class JessicaJonesTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void NoCardsPlayed_PowerIsNine(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                5,
                side == Side.Top ? new[] { ("Jessica Jones", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Jessica Jones", column) }
                    : new (string, Column)[] { }
            );

            var engine = new Engine(new NullLogger());
            game = engine.PlaySingleTurn(game);

            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var jessicaJones = game[column][side][0];
            Assert.That(jessicaJones.Name, Is.EqualTo("Jessica Jones"));

            Assert.That(jessicaJones.Power, Is.EqualTo(9));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void CardPlayed_PowerRemainsAtFive(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                5,
                side == Side.Top ? new[] { ("Jessica Jones", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Jessica Jones", column) }
                    : new (string, Column)[] { }
            );

            game = TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top ? new[] { ("Misty Knight", column) } : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count, Is.EqualTo(2));

            var jessicaJones = game[column][side][0];
            Assert.That(jessicaJones.Name, Is.EqualTo("Jessica Jones"));

            Assert.That(jessicaJones.Power, Is.EqualTo(5));
        }
    }
}
