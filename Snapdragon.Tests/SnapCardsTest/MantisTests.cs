namespace Snapdragon.Tests.SnapCardsTest
{
    public class MantisTests
    {
        [Test]
        public void OpponentPlaysCardSameLocation_CopiesOpponentCardToHand()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Middle) }
                );

            Assert.That(game[Side.Top].Hand.Count, Is.EqualTo(1));
            Assert.That(game[Side.Top].Hand[0].Name, Is.EqualTo("Ant Man"));
        }

        [Test]
        public void OpponentPlaysCardDifferentLocation_DoesNotCopyToHand()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(
                    new[] { ("Mantis", Column.Middle) },
                    new[] { ("Ant Man", Column.Right) }
                );

            Assert.That(game[Side.Top].Hand, Is.Empty);
        }

        [Test]
        public void PlaysTwoCardsToSameLocation_DoesNotCopyToHand()
        {
            var game = TestHelpers
                .NewGame()
                .PlayCards(Side.Top, Column.Middle, "Mantis", "Ant Man");

            Assert.That(game[Side.Top].Hand, Is.Empty);
        }
    }
}
