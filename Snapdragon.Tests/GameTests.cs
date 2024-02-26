using Snapdragon.Events;
using Snapdragon.PlayerActions;

namespace Snapdragon.Tests
{
    public class GameTests
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
        public async Task StartTurn_GivesPlayerEnergy(int turn)
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGame(engine);

            for (var i = 0; i < turn - 1; i++)
            {
                game = await game.PlaySingleTurn();
            }

            game = await game.StartNextTurn();

            Assert.That(game.Top.Energy, Is.EqualTo(turn));
            Assert.That(game.Bottom.Energy, Is.EqualTo(turn));
        }

        [Test]
        [TestCase(1, true, false, false)]
        [TestCase(2, true, true, false)]
        [TestCase(3, true, true, true)]
        [TestCase(4, true, true, true)]
        [TestCase(5, true, true, true)]
        [TestCase(6, true, true, true)]
        [TestCase(7, true, true, true)]
        public async Task StartTurn_RevealsLocationForTurn(
            int turn,
            bool leftRevealed,
            bool middleRevealed,
            bool rightRevealed
        )
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGame(engine);

            for (var i = 0; i < turn - 1; i++)
            {
                game = await game.PlaySingleTurn();
            }

            game = await game.StartNextTurn();

            Assert.That(game.Left.Revealed, Is.EqualTo(leftRevealed));
            Assert.That(game.Middle.Revealed, Is.EqualTo(middleRevealed));
            Assert.That(game.Right.Revealed, Is.EqualTo(rightRevealed));
        }

        [Test]
        [TestCase(1, Column.Left)]
        [TestCase(2, Column.Middle)]
        [TestCase(3, Column.Right)]
        public async Task StartTurn_RaisesRevealLocationEvent(int turn, Column column)
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGame(engine);

            for (var i = 0; i < turn - 1; i++)
            {
                game = await game.PlaySingleTurn();
            }

            game = await game.StartNextTurn();

            var lastReveal = game.PastEvents.OfType<LocationRevealedEvent>().Last();

            Assert.That(lastReveal.Location.Column, Is.EqualTo(column));
        }

        [Test]
        public async Task StartTurn_DrawsCard()
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

            game = await game.StartNextTurn();

            // Note: Start of game gives three cards, as tested elsewhere.
            Assert.That(game.Top.Hand.Count, Is.EqualTo(4));
        }

        #endregion

        #region PlaySingleTurn

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public async Task PlaySingleTurn_PutsCardIntoColumn(Column column)
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

            game = await game.PlaySingleTurn();

            Assert.That(game[column].TopPlayerCards.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public async Task PlaySingleTurn_SetsCardRevealed(Column column)
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

            game = await game.PlaySingleTurn();

            Assert.That(game[column].TopPlayerCards[0].State, Is.EqualTo(CardState.InPlay));
        }

        [Test]
        [TestCase(Side.Top, Column.Left)]
        [TestCase(Side.Top, Column.Middle)]
        [TestCase(Side.Top, Column.Right)]
        [TestCase(Side.Bottom, Column.Left)]
        [TestCase(Side.Bottom, Column.Middle)]
        [TestCase(Side.Bottom, Column.Right)]
        public async Task PlaySingleTurn_SetsLeaderAsFirstRevealedForNextTurn(
            Side side,
            Column column
        )
        {
            var engine = new Engine(new NullLogger());
            var topPlayerController = new TestPlayerController();
            var bottomPlayerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration(
                "Top",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                topPlayerController
            );
            var bottomPlayerConfig = new PlayerConfiguration(
                "Bottom",
                new Deck([Cards.OneOne, Cards.TwoTwo, Cards.TwoThree]),
                bottomPlayerController
            );

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            if (side == Side.Top)
            {
                topPlayerController.Actions = new List<IPlayerAction>
                {
                    new PlayCardAction(Side.Top, game.Top.Hand[0], column)
                };
            }
            else
            {
                bottomPlayerController.Actions = new List<IPlayerAction>
                {
                    new PlayCardAction(Side.Bottom, game.Bottom.Hand[0], column)
                };
            }

            game = await game.PlaySingleTurn();

            Assert.That(game.FirstRevealed, Is.EqualTo(side));
        }

        [Test]
        [TestCase(Column.Left)]
        [TestCase(Column.Middle)]
        [TestCase(Column.Right)]
        public async Task PlaySingleTurn_RaisesCardRevealedEvent(Column column)
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

            game = await game.PlaySingleTurn();

            var cardRevealedEvent = game.PastEvents.SingleOrDefault(e =>
                e.Type == EventType.CardRevealed
                && ((CardRevealedEvent)e).Card.Definition == Cards.OneOne
            );

            Assert.That(cardRevealedEvent, Is.Not.Null);
        }

        #endregion

        #region PlayGame

        [Test]
        public async Task PlayGame_PlaysSixTurnsByDefault()
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGame(engine);

            game = await game.PlayGame();

            Assert.That(game.Turn, Is.EqualTo(6));
        }

        [Test]
        public async Task PlayGame_GameOverIsTrue()
        {
            var engine = new Engine(new NullLogger());
            var game = GetInitialGame(engine);

            game = await game.PlayGame();

            Assert.That(game.GameOver, Is.True);
        }

        #endregion

        #region Helper Functions

        private static Game GetInitialGame(Engine engine)
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
