namespace Snapdragon
{
    public class MonteCarloSearchController : IPlayerController
    {
        /// <summary>
        /// The number of random simulations to run,
        /// for each possible move set.
        /// </summary>
        private int simulationCount;

        public MonteCarloSearchController(int simulationCount)
        {
            this.simulationCount = simulationCount;
        }

        public IReadOnlyList<IPlayerAction> GetActions(Game game, Side side)
        {
            IReadOnlyList<IPlayerAction> noActions = new List<IPlayerAction>();
            var bestActionSets = new List<IReadOnlyList<IPlayerAction>> { noActions };
            var bestActionVictories = 0;

            var possibleActionLists = ControllerUtilities.GetPossibleActionSets(game, side);

            // This honestly probably doesn't matter, but I'm trying to avoid
            // instantiating all of these objects at once.
            foreach (var possibleActions in possibleActionLists)
            {
                var victories = 0;

                for (var i = 0; i < this.simulationCount; i++)
                {
                    var endState = SimulateToEnd(game, side, possibleActions);

                    if (endState.GetLeader() == side)
                    {
                        victories += 1;
                    }
                }

                if (victories > bestActionVictories)
                {
                    bestActionVictories = victories;
                    bestActionSets.Clear();
                    bestActionSets.Add(possibleActions);
                }
                else if (victories == bestActionVictories)
                {
                    // Avoid stacking up a bunch of possibilities
                    // that are all equally bad at the beginning.
                    if (victories > 0)
                    {
                        bestActionSets.Add(possibleActions);
                    }
                }
            }

            return Random.Of(bestActionSets);
        }

        private static Game SimulateToEnd(
            Game game,
            Side side,
            IReadOnlyList<IPlayerAction> actions
        )
        {
            var playerController = new TemporaryPlayerController(actions);
            IPlayerController topController;
            IPlayerController bottomController;

            switch (side)
            {
                case Side.Top:
                    topController = playerController;
                    bottomController = new RandomPlayerController();
                    break;
                case Side.Bottom:
                    topController = new RandomPlayerController();
                    bottomController = playerController;
                    break;
                default:
                    throw new NotImplementedException();
            }

            game = game with
            {
                Top = game.Top.WithController(topController),
                Bottom = game.Bottom.WithController(bottomController),
                Logger = new NullLogger()
            };

            game = game.PlayAlreadyStartedTurn();

            while (!game.GameOver)
            {
                game = game.PlaySingleTurn();
            }

            return game;
        }
    }
}
