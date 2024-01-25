using Snapdragon.Events;
using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public class EngineTests
    {
        [SetUp]
        public void Setup() { }

        #region CreateGame

        [Test]
        public void CreateGame_TurnIsZero()
        {
            var engine = new Engine(new NullLogger());

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([]),
                new NullPlayerController()
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);

            Assert.That(game.Turn, Is.EqualTo(0));
        }

        [Test]
        public void CreateGame_PlayerConfigurationsAsExpected()
        {
            var engine = new Engine(new NullLogger());

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([]),
                new NullPlayerController()
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);

            Assert.That(game.Top.Configuration, Is.EqualTo(topPlayerConfig));
            Assert.That(game.Bottom.Configuration, Is.EqualTo(bottomPlayerConfig));
        }

        [Test]
        public void CreateGame_PlayersDrawThreeCards()
        {
            var engine = new Engine(new NullLogger());

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                new NullPlayerController()
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([Cards.OneTwo, Cards.OneThree, Cards.TwoOne]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);

            Assert.That(game.Top.Hand.Count, Is.EqualTo(3));
            Assert.That(game.Bottom.Hand.Count, Is.EqualTo(3));
        }

        #endregion

        #region StartTurn

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void StartTurn_GivesPlayerEnergy(int turn)
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGameState(engine);

            for (var i = 0; i < turn - 1; i++)
            {
                game = engine.PlaySingleTurn(game);
            }

            game = engine.StartNextTurn(game);

            Assert.That(game.Top.Energy, Is.EqualTo(turn));
            Assert.That(game.Bottom.Energy, Is.EqualTo(turn));
        }

        [Test]
        public void StartTurn_DrawsCard()
        {
            var engine = new Engine(new NullLogger());
            var playerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree, Cards.OneTwo]),
                playerController
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            game = engine.StartNextTurn(game);

            // Note: Start of game gives three cards, as tested elsewhere.
            Assert.That(game.Top.Hand.Count, Is.EqualTo(4));
        }

        #endregion

        #region PlaySingleTurn

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void PlaySingleTurn_PutsCardIntoColumn(Column column)
        {
            var engine = new Engine(new NullLogger());
            var playerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                playerController
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            // TODO - Play a card
            playerController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, game.Top.Hand[0], column)
            };

            game = engine.PlaySingleTurn(game);

            Assert.That(game[column].TopPlayerCards.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void PlaySingleTurn_SetsCardRevealed(Column column)
        {
            var engine = new Engine(new NullLogger());
            var playerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                playerController
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            // TODO - Play a card
            playerController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, game.Top.Hand[0], column)
            };

            game = engine.PlaySingleTurn(game);

            Assert.That(game[column].TopPlayerCards[0].State, Is.EqualTo(CardState.InPlay));
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public void PlaySingleTurn_RaisesCardRevealedEvent(Column column)
        {
            var engine = new Engine(new NullLogger());
            var playerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                playerController
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            // TODO - Play a card
            playerController.Actions = new List<IPlayerAction>
            {
                new PlayCardAction(Side.Top, game.Top.Hand[0], column)
            };

            game = engine.PlaySingleTurn(game);

            var cardRevealedEvent = game.PastEvents.SingleOrDefault(e =>
                e.Type == EventType.CardRevealed
                && ((CardRevealedEvent)e).Card.Definition == Cards.OneOne
            );

            Assert.That(cardRevealedEvent, Is.Not.Null);
        }

        #endregion

        #region Helper Functions

        private static GameState GetInitialGameState(Engine engine)
        {
            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([]),
                new NullPlayerController()
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([]),
                new NullPlayerController()
            );

            return engine.CreateGame(topPlayerConfig, bottomPlayerConfig);
        }

        #endregion
    }
}
