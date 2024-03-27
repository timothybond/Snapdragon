using System.Collections.Immutable;

namespace Snapdragon.Tests
{
    public class GameKernelTests
    {
        [Test]
        [TestCase(Side.Top, "Ka-Zar")]
        [TestCase(Side.Bottom, "Vulture")]
        public void DrawCard_AddsCardToHand(Side side, string cardName)
        {
            // See BuildKernel for card order
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            switch (side)
            {
                case Side.Top:
                    Assert.That(kernel.TopHand, Has.Exactly(1).Items);
                    Assert.That(kernel.Cards[kernel.TopHand[0]].Name, Is.EqualTo(cardName));
                    break;
                case Side.Bottom:
                    Assert.That(kernel.BottomHand, Has.Exactly(1).Items);
                    Assert.That(kernel.Cards[kernel.BottomHand[0]].Name, Is.EqualTo(cardName));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [Test]
        [TestCase(Side.Top, "Squirrel Girl")]
        [TestCase(Side.Bottom, "Human Torch")]
        public void DrawCard_RemovesCardFromDeck(Side side, string nextCardName)
        {
            // See BuildKernel for card order
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            switch (side)
            {
                case Side.Top:
                    Assert.That(kernel.TopLibrary, Has.Exactly(11).Items);
                    Assert.That(kernel.Cards[kernel.TopLibrary[0]].Name, Is.EqualTo(nextCardName));
                    break;
                case Side.Bottom:
                    Assert.That(kernel.BottomLibrary, Has.Exactly(11).Items);
                    Assert.That(
                        kernel.Cards[kernel.BottomLibrary[0]].Name,
                        Is.EqualTo(nextCardName)
                    );
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [Test]
        [TestCase(Side.Top)]
        [TestCase(Side.Bottom)]
        public void DrawCard_UpdatesStateToInHand(Side side)
        {
            var kernel = BuildKernel();

            kernel = kernel.DrawCard(side);

            switch (side)
            {
                case Side.Top:
                    Assert.That(kernel.CardStates[kernel.TopHand[0]], Is.EqualTo(CardState.InHand));
                    break;
                case Side.Bottom:
                    Assert.That(
                        kernel.CardStates[kernel.BottomHand[0]],
                        Is.EqualTo(CardState.InHand)
                    );
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static GameKernel BuildKernel()
        {
            var kaZarDeck = GetDeck(
                "Ka-Zar",
                "Squirrel Girl",
                "Blue Marvel",
                "Ant Man",
                "Hawkeye",
                "Elektra",
                "Rocket Raccoon",
                "Okoye",
                "Human Torch",
                "Iron Fist",
                "Nightcrawler",
                "Misty Knight"
            );

            var moveDeck = GetDeck(
                "Vulture",
                "Human Torch",
                "Doctor Strange",
                "Multiple Man",
                "Kraven",
                "Iron Fist",
                "Cloak",
                "Heimdall",
                "Medusa",
                "Iron Man",
                "Hawkeye",
                "Nightcrawler"
            );

            var topConfig = new PlayerConfiguration(
                "Ka-Zar",
                kaZarDeck,
                new MonteCarloSearchController(5)
            );
            var bottomConfig = new PlayerConfiguration(
                "Move",
                moveDeck,
                new MonteCarloSearchController(5)
            );

            var topPlayer = new Player(topConfig, Side.Top, false);
            var bottomPlayer = new Player(bottomConfig, Side.Bottom, false);

            return GameKernel.FromPlayers(topPlayer, bottomPlayer);
        }

        private static Deck GetDeck(params string[] cardNames)
        {
            if (cardNames.Length != 12)
            {
                throw new ArgumentException("Must specify 12 cards.");
            }

            return new Deck(cardNames.Select(name => SnapCards.ByName[name]).ToImmutableList());
        }
    }
}
