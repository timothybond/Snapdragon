namespace Snapdragon.Tests.SnapCardsTest
{
    public class IronheartTests
    {
        [Test]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Bottom, Column.Middle)]
        public void DoesNotAddToSelf(Side side, Column column)
        {
            var game = TestHelpers.PlayCards(6, side, new[] { ("Ironheart", column) });

            var engine = new Engine(new NullLogger());
            game = game.PlaySingleTurn();
            Assert.That(game[column][side].Count, Is.EqualTo(1));

            var ironheart = game[column][side][0];
            Assert.That(ironheart.Name, Is.EqualTo("Ironheart"));

            Assert.That(ironheart.Power, Is.EqualTo(0));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void AddsToOtherCardOnce(Side side)
        {
            var game = TestHelpers.PlayCards(5, side, new[] { ("Hawkeye", Column.Left) });
            game = TestHelpers.PlayCards(game, 6, side, new[] { ("Ironheart", Column.Right) });

            var hawkeye = game[Column.Left][side].Single();
            Assert.That(hawkeye.Name, Is.EqualTo("Hawkeye"));

            Assert.That(hawkeye.Power, Is.EqualTo(3));
        }

        // Note that all these cards have power 1 (when played under these conditions)
        [Test]
        [TestCase(Side.Top, "Hawkeye")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man")]
        [TestCase(Side.Top, "Hawkeye", "Ant Man", "Wasp")]
        [TestCase(Side.Bottom, "Hawkeye")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man")]
        [TestCase(Side.Bottom, "Hawkeye", "Ant Man", "Wasp")]
        public void AddsToUpToThreeCards(Side side, params string[] cardNames)
        {
            var game = TestHelpers.PlayCards(
                5,
                side,
                cardNames.Select(name => (name, Column.Left)).ToArray()
            );
            game = TestHelpers.PlayCards(game, 6, side, new[] { ("Ironheart", Column.Right) });

            Assert.That(game[Column.Left][side].Count, Is.EqualTo(cardNames.Length));

            for (var i = 0; i < cardNames.Length; i++)
            {
                Assert.That(game[Column.Left][side][i].Power, Is.EqualTo(3));
            }
        }

        // Note that all these cards have power 1 (when played under these conditions)
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotAddToFourthCard(Side side)
        {
            var cardNames = new[] { "Ant Man", "Wasp", "Hawkeye", "Squirrel" };

            var game = TestHelpers.PlayCards(
                5,
                side,
                cardNames.Select(name => (name, Column.Left)).ToArray()
            );
            game = TestHelpers.PlayCards(game, 6, side, new[] { ("Ironheart", Column.Right) });

            Assert.That(game[Column.Left][side].Count, Is.EqualTo(4));

            var cardsWithThreePower = game[Column.Left][side].Where(c => c.Power == 3);
            var cardsWithOnePower = game[Column.Left][side].Where(c => c.Power == 1);

            Assert.That(cardsWithThreePower.Count(), Is.EqualTo(3));
            Assert.That(cardsWithOnePower.Count(), Is.EqualTo(1));
        }
    }
}
