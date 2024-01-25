namespace Snapdragon.Tests
{
    public class EngineTests
    {
        private static readonly CardDefinition OneOne = new CardDefinition("OneOne", 1, 1);
        private static readonly CardDefinition OneTwo = new CardDefinition("OneTwo", 1, 2);
        private static readonly CardDefinition OneThree = new CardDefinition("OneThree", 1, 3);

        private static readonly CardDefinition TwoOne = new CardDefinition("TwoOne", 2, 1);
        private static readonly CardDefinition TwoTwo = new CardDefinition("TwoTwo", 2, 2);
        private static readonly CardDefinition TwoThree = new CardDefinition("TwoThree", 2, 3);

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreateGame_CreatesExpectedState()
        {
            var engine = new Engine(new NullLogger());

            var topPlayerConfig = new PlayerConfiguration("Top", new Deck([]), new NullPlayerController());
            var bottomPlayerConfig = new PlayerConfiguration("Bottom", new Deck([]), new NullPlayerController());

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig);

            Assert.That(game.Turn, Is.EqualTo(0));
            Assert.That(game.Top.Configuration, Is.EqualTo(topPlayerConfig));
            Assert.That(game.Bottom.Configuration, Is.EqualTo(bottomPlayerConfig));
        }

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
        public void ProcessTurn_PutsCardIntoPlay()
        {
            var engine = new Engine(new NullLogger());
            var playerController = new TestPlayerController();

            var topPlayerConfig = new PlayerConfiguration("Top", new Deck([OneOne, TwoTwo, TwoThree]), playerController);
            var bottomPlayerConfig = new PlayerConfiguration("Bottom", new Deck([]), new NullPlayerController());

            var game = engine.CreateGame(topPlayerConfig, bottomPlayerConfig, false);

            // TODO - Play a card
            // playerController.Actions = new List<IPlayerAction> { new PlayCardAction() }


        }

        private static GameState GetInitialGameState(Engine engine)
        {
            var topPlayerConfig = new PlayerConfiguration("Top", new Deck([]), new NullPlayerController());
            var bottomPlayerConfig = new PlayerConfiguration("Bottom", new Deck([]), new NullPlayerController());

            return engine.CreateGame(topPlayerConfig, bottomPlayerConfig);
        }
    }
}