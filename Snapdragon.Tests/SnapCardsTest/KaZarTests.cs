using System.Data.Common;
using System.Drawing;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class KaZarTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void AddsToOneCostCards(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Misty Knight", Column.Right)
            };

            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? cardsToPlay : noCards,
                side == Side.Bottom ? cardsToPlay : noCards
            );

            Assert.That(game[Column.Right][side].Count == 1);

            var mistyKnight = game[Column.Right][side][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(mistyKnight.PowerAdjustment, Is.EqualTo(1));
            Assert.That(mistyKnight.AdjustedPower, Is.EqualTo(3));
        }
    }
}
