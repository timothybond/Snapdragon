using System.Data.Common;
using System.Drawing;

namespace Snapdragon.Tests.SnapCardsTest
{
    public class BlueMarvelTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DoesNotAddToSelf(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Blue Marvel", Column.Left),
                ("Misty Knight", Column.Right)
            };

            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? cardsToPlay : noCards,
                side == Side.Bottom ? cardsToPlay : noCards
            );

            Assert.That(game[Column.Right][side].Count == 1);

            var blueMarvel = game[Column.Left][side][0];
            Assert.That(blueMarvel.Name, Is.EqualTo("Blue Marvel"));

            Assert.That(blueMarvel.PowerAdjustment, Is.Null);
            Assert.That(blueMarvel.AdjustedPower, Is.EqualTo(3));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void AddsToOtherCard(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Blue Marvel", Column.Left),
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
        public void DoesNotAddToOpponentCard(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };

            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Blue Marvel", Column.Left),
            };
            (string CardName, Column Column)[] opponentCardsToPlay = new[]
            {
                ("Misty Knight", Column.Right)
            };

            var game = TestHelpers.PlayCards(
                6,
                side == Side.Top ? cardsToPlay : opponentCardsToPlay,
                side == Side.Bottom ? cardsToPlay : opponentCardsToPlay
            );

            Assert.That(game[Column.Right][side.Other()].Count == 1);

            var mistyKnight = game[Column.Right][side.Other()][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(mistyKnight.PowerAdjustment, Is.Null);
            Assert.That(mistyKnight.AdjustedPower, Is.EqualTo(2));
        }
    }
}
