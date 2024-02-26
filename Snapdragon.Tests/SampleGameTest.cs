using System.Collections.Immutable;

namespace Snapdragon.Tests
{
    public class SampleGameTest
    {
        // This is just a test to allow me to walk through a sample game
        // and make sure everything is working correctly.
        [Test]
        public async Task PlayGame()
        {
            var logger = new TestLogger();

            var game = GetNewGame(logger);
            game = await game.PlayGame();

            Assert.Pass(logger.ToString());
        }

        // This sees what happens if we run the same deck against itself with two
        // different controllers (differing only by their simulation count).
        // It mostly serves as a sanity check that "better" Monte Carlo controllers
        // actually perform consistently better.
        //
        // When used in mirror matches with the Ka-Zar deck, performance seemed to
        // basically level off around 5 simulations, but with the "Move" deck it
        // still had an edge up to 20 but not thereafter. This probably speaks to
        // the fact that the "Move" strategy is more complex to execute, and also
        // that it literally has more possible options (playing Cloak often
        // causes an explosion of potential action sets on the next turn).

        // Disabled as a test because of long execution times.
        //[Test]
        //[TestCase(1, 5)]
        //[TestCase(5, 10)]
        //[TestCase(10, 20)]
        //[TestCase(20, 50)]
        public async Task ComparePerformanceByControllers(int lowSimCount, int highSimCount)
        {
            if (lowSimCount >= highSimCount)
            {
                Assert.Fail("First argument must be lower than second argument.");
            }

            var totalGames = 100;
            var wins = 0;

            await Enumerable
                .Range(0, totalGames)
                .ForEachAsync(
                    async _ =>
                    {
                        var game = GetMirrorGame(new NullLogger(), highSimCount, lowSimCount);
                        game = await game.PlayGame();

                        return game.GetCurrentScores();
                    },
                    (_, scores) =>
                    {
                        if (scores.Leader == Side.Top)
                        {
                            Interlocked.Increment(ref wins);
                        }
                    }
                );

            Assert.Pass($"Top wins: {wins}/{totalGames}");
        }

        // Most of these test cases are commented out because this test is pretty slow,
        // so we don't want to run it every time we run all of the tests.
        [Test]
        [TestCase(1)]
        //[TestCase(5)]
        //[TestCase(10)]
        //[TestCase(20)]
        //[TestCase(50)]
        //[TestCase(100)]
        //[TestCase(200)]
        public async Task GetAverageScores(int monteCarloSimulationCount)
        {
            var scoreList = new List<CurrentScores>();

            /* From a previous run of 100 games each:
             *
             *   1 sim:  29.545
             *   5 sims: 32.995
             *  10 sims: 35.440
             *  20 sims: 35.665
             *  50 sims: 37.255
             * 100 sims: 38.165
             * 200 sims: 37.185
             */

            await Enumerable
                .Range(0, 20)
                .ForEachAsync(
                    async _ =>
                    {
                        var game = GetNewGame(new NullLogger(), monteCarloSimulationCount);
                        game = await game.PlayGame();

                        return game.GetCurrentScores();
                    },
                    (_, scores) =>
                    {
                        lock (scoreList)
                        {
                            scoreList.Add(scores);
                        }
                    }
                );

            var averagePlayerScore =
                scoreList.Sum(s =>
                    (
                        s.Left.Top
                        + s.Left.Bottom
                        + s.Middle.Top
                        + s.Middle.Bottom
                        + s.Right.Top
                        + s.Right.Bottom
                    ) / 2.0
                ) / scoreList.Count;

            Assert.Pass(
                $"{monteCarloSimulationCount} simulations: {averagePlayerScore} average score."
            );
        }

        private static Game GetNewGame(IGameLogger logger, int monteCarloSimulationCount = 5)
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
                new MonteCarloSearchController(monteCarloSimulationCount)
            );
            var bottomConfig = new PlayerConfiguration(
                "Move",
                moveDeck,
                new MonteCarloSearchController(monteCarloSimulationCount)
            );

            var engine = new Engine(logger);
            return engine.CreateGame(topConfig, bottomConfig);
        }

        /// <summary>
        /// Same as <see cref="GetNewGame"/> except one deck is used twice,
        /// and the controllers can have different simulation counts.
        /// </summary>
        private static Game GetMirrorGame(
            IGameLogger logger,
            int topSimuationCount,
            int bottomSimulationCount
        )
        {
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
                "Top",
                moveDeck,
                new MonteCarloSearchController(topSimuationCount)
            );
            var bottomConfig = new PlayerConfiguration(
                "Bottom",
                moveDeck,
                new MonteCarloSearchController(bottomSimulationCount)
            );

            var engine = new Engine(logger);
            return engine.CreateGame(topConfig, bottomConfig);
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
