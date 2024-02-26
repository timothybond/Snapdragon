namespace Snapdragon.Tests.SnapCardsTest
{
    public class SpectrumTests
    {
        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task DoesNotAddToSelf(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Misty Knight", Column.Middle)
            };

            // Put other cards on both sides
            var game = await TestHelpers.PlayCards(5, cardsToPlay, cardsToPlay);

            (string CardName, Column Column)[] playSpectrum = new[] { ("Spectrum", Column.Right), };

            game = await TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top ? playSpectrum : noCards,
                side == Side.Bottom ? playSpectrum : noCards
            );

            Assert.That(game[Column.Right][side].Count, Is.EqualTo(1));

            var spectrum = game[Column.Right][side][0];
            Assert.That(spectrum.Name, Is.EqualTo("Spectrum"));

            Assert.That(spectrum.Power, Is.EqualTo(7));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task DoesNotAddToCardWithoutOngoingAbility(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Misty Knight", Column.Middle)
            };

            // Put other cards on both sides
            var game = await TestHelpers.PlayCards(5, cardsToPlay, cardsToPlay);

            (string CardName, Column Column)[] playSpectrum = new[] { ("Spectrum", Column.Right), };

            game = await TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top ? playSpectrum : noCards,
                side == Side.Bottom ? playSpectrum : noCards
            );

            Assert.That(game[Column.Middle][side].Count, Is.EqualTo(1));

            var mistyKnight = game[Column.Middle][side][0];
            Assert.That(mistyKnight.Name, Is.EqualTo("Misty Knight"));

            Assert.That(mistyKnight.Power, Is.EqualTo(2));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task AddsToCardWithOngoingAbility(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Misty Knight", Column.Middle)
            };

            // Put other cards on both sides
            var game = await TestHelpers.PlayCards(5, cardsToPlay, cardsToPlay);

            (string CardName, Column Column)[] playSpectrum = new[] { ("Spectrum", Column.Right), };

            game = await TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top ? playSpectrum : noCards,
                side == Side.Bottom ? playSpectrum : noCards
            );

            Assert.That(game[Column.Left][side].Count, Is.EqualTo(1));

            var kaZar = game[Column.Left][side][0];
            Assert.That(kaZar.Name, Is.EqualTo("Ka-Zar"));

            Assert.That(kaZar.Power, Is.EqualTo(6));
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public async Task DoesNotAddToEnemyCardWithOngoingAbility(Side side)
        {
            var noCards = new (string CardName, Column Column)[] { };
            (string CardName, Column Column)[] cardsToPlay = new[]
            {
                ("Ka-Zar", Column.Left),
                ("Misty Knight", Column.Middle)
            };

            // Put other cards on both sides
            var game = await TestHelpers.PlayCards(5, cardsToPlay, cardsToPlay);

            (string CardName, Column Column)[] playSpectrum = new[] { ("Spectrum", Column.Right), };

            game = await TestHelpers.PlayCards(
                game,
                6,
                side == Side.Top ? playSpectrum : noCards,
                side == Side.Bottom ? playSpectrum : noCards
            );

            Assert.That(game[Column.Left][side.Other()].Count, Is.EqualTo(1));

            var kaZar = game[Column.Left][side.Other()][0];
            Assert.That(kaZar.Name, Is.EqualTo("Ka-Zar"));

            Assert.That(kaZar.Power, Is.EqualTo(4));
        }
    }
}
