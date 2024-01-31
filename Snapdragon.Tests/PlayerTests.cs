namespace Snapdragon.Tests
{
    public class PlayerTests
    {
        [Test]
        public void StartOfGameConstructor_SetsEnergyToZero()
        {
            var config = new PlayerConfiguration(
                "Test",
                new Deck([Cards.OneOne, Cards.TwoTwo]),
                new NullPlayerController()
            );
            var player = new Player(config, Side.Top);

            Assert.That(player.Energy, Is.EqualTo(0));
        }

        [Test]
        public void StartOfGameConstructor_LeavesHandEmpty()
        {
            var config = new PlayerConfiguration(
                "Test",
                new Deck([Cards.OneOne, Cards.TwoTwo]),
                new NullPlayerController()
            );
            var player = new Player(config, Side.Top);

            Assert.That(player.Hand, Is.Empty);
        }

        [Test]
        public void DrawCard_AddsNextCardToHand()
        {
            var config = new PlayerConfiguration(
                "Test",
                new Deck([Cards.OneOne, Cards.TwoTwo]),
                new NullPlayerController()
            );
            var player = new Player(config, Side.Top, false).DrawCard();

            Assert.That(player.Hand.Count, Is.EqualTo(1));
            Assert.That(player.Hand[0].Definition, Is.EqualTo(Cards.OneOne));
        }

        [Test]
        public void DrawCard_DrawsToMaxOfSeven()
        {
            var config = new PlayerConfiguration(
                "Test",
                new Deck(
                    [
                        Cards.OneOne,
                        Cards.OneTwo,
                        Cards.OneThree,
                        Cards.TwoOne,
                        Cards.TwoTwo,
                        Cards.TwoThree,
                        Cards.ThreeOne,
                        Cards.ThreeTwo,
                        Cards.ThreeThree
                    ]
                ),
                new NullPlayerController()
            );

            var player = new Player(config, Side.Top, false);

            for (var i = 0; i < 20; i++)
            {
                player = player.DrawCard();
            }

            Assert.That(player.Hand.Count, Is.EqualTo(7));
            Assert.That(player.Library.Count, Is.EqualTo(2));
        }
    }
}
