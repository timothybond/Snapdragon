namespace Snapdragon.Tests.SnapCardsTest
{
    public class MantisTests
    {
        [Test]
        public void OpponentPlaysCardSameLocation_CopiesOpponentCard()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Middle) }
                );

            Assert.That(game[Column.Middle][Side.Top].Count, Is.EqualTo(2));
            Assert.That(game[Column.Middle][Side.Top].Last().Name, Is.EqualTo("Ant Man"));
        }

        [Test]
        public void OpponentPlaysCardDifferentLocation_DoesNotCopy()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Right) }
                );

            Assert.That(game[Column.Middle][Side.Top].Count, Is.EqualTo(1));
            Assert.That(game[Column.Left][Side.Top].Count, Is.EqualTo(0));
            Assert.That(game[Column.Right][Side.Top].Count, Is.EqualTo(0));
        }

        [Test]
        public void PlaysTwoCardsToSameLocation_DoesNotCopy()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(Side.Top, Column.Middle, "Mantis", "Ant Man");

            Assert.That(game[Column.Middle][Side.Top].Count, Is.EqualTo(2));
        }
    }
}
