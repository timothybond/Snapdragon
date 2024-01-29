using System.Data.Common;
using System.Drawing;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class KaZarTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotAddToSelf(Side side)
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

            var kaZar = game[Column.Left][side][0];
            Assert.That(kaZar.Name, Is.EqualTo("Ka-Zar"));

            Assert.That(kaZar.PowerAdjustment, Is.Null);
            Assert.That(kaZar.AdjustedPower, Is.EqualTo(4));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void AddsToOneCostCard(Side side)
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

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotAddToTwoCostCard(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Star-Lord", Column.Right)
            };

            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? cardsToPlay : noCards,
                side == Side.Bottom ? cardsToPlay : noCards
            );

            Assert.That(game[Column.Right][side].Count == 1);

            var starLord = game[Column.Right][side][0];
            Assert.That(starLord.Name, Is.EqualTo("Star-Lord"));

            Assert.That(starLord.PowerAdjustment, Is.Null);
            Assert.That(starLord.AdjustedPower, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotAddToOpponentCard(Side side)
        {
            (string CardName, Column Column)[] cardsToPlay = new[] { ("Ka-Zar", Column.Left), };
            (string CardName, Column Column)[] opponentCardsToPlay = new[]
            {
                ("Misty Knight", Column.Right)
            };

            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? cardsToPlay : opponentCardsToPlay,
                side == Side.Bottom ? cardsToPlay : opponentCardsToPlay
            );

            Assert.That(game[Column.Right][side.OtherSide()].Count == 1);

            var mistyKnight = game[Column.Right][side.OtherSide()][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(mistyKnight.PowerAdjustment, Is.Null);
            Assert.That(mistyKnight.AdjustedPower, Is.EqualTo(2));
        }
    }
}
