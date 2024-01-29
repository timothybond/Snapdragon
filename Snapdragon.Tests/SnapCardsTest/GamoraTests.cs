namespace Snapdragon.Tests.SnapCardsTest
{
    public class GamoraTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardAlone_PowerIsSeven(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? new[] { ("Gamora", column) } : new (string, Column)[] { },
                side == Side.Bottom ? new[] { ("Gamora", column) } : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 1);

            var gamora = game[column][side][0];
            Assert.That(gamora.Name, Is.EqualTo("Gamora"));

            Assert.That(gamora.Power, Is.EqualTo(7));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardWithAnotherCardOnSameSide_PowerIsSeven(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top
                    ? new[] { ("Gamora", column), ("Misty Knight", column) }
                    : new (string, Column)[] { },
                side == Side.Bottom
                    ? new[] { ("Gamora", column), ("Misty Knight", column) }
                    : new (string, Column)[] { }
            );

            Assert.That(game[column][side].Count == 2);

            var gamora = game[column][side][0];
            Assert.That(gamora.Name, Is.EqualTo("Gamora"));

            Assert.That(gamora.Power, Is.EqualTo(7));
        }

        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void PlayCardWithAnotherCardOnOpposingSide_PowerIsTwelve(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top
                    ? new[] { ("Gamora", column) }
                    : new[] { ("Misty Knight", column) },
                side == Side.Bottom
                    ? new[] { ("Gamora", column) }
                    : new[] { ("Misty Knight", column) }
            );

            Assert.That(game[column][side].Count == 1);

            var gamora = game[column][side][0];
            Assert.That(gamora.Name, Is.EqualTo("Gamora"));

            Assert.That(gamora.Power, Is.EqualTo(12));
        }
    }
}
