using Snapdragon.Events;
using System.Collections.Immutable;

namespace Snapdragon
{
    public class Engine
    {
        private readonly IGameLogger logger;

        public Engine(IGameLogger logger)
        {
            this.logger = logger;
        }

        public GameState CreateGame(PlayerConfiguration topPlayer, PlayerConfiguration bottomPlayer, bool shuffle = true)
        {
            // TODO: Specify different Locations, with effects
            return new GameState(
                0,
                new Location("Left", Column.Left),
                new Location("Middle", Column.Middle),
                new Location("Right", Column.Right),
                topPlayer.ToState(Side.Top, shuffle),
                bottomPlayer.ToState(Side.Bottom, shuffle),
                [],
                []);
        }

        /// <summary>
        /// Processes the beginning of a Turn.
        /// 
        /// Used inside <see cref="PlaySingleTurn(GameState)"/>, but exposed here for unit-testing purposes.
        /// </summary>
        /// <param name="game">The <see cref="GameState"/> at the end of the previous Turn.</param>
        /// <returns>The <see cref="GameState"/> at the start of the new Turn, before any <see cref="PlayerConfiguration"/> actions.</returns>
        public GameState StartNextTurn(GameState game)
        {
            // Note the check for Games going over is in PlaySingleTurn
            var currentTurn = game.Turn + 1;

            // Give each Player an amount of energy equal to the turn count
            var topPlayer = game.Top with { Energy = currentTurn };
            var bottomPlayer = game.Bottom with { Energy = currentTurn };

            game = game with { Top = topPlayer, Bottom = bottomPlayer, Turn = currentTurn };

            // Raise an event for the start of the turn
            game = game.WithEvent(new StartTurnEvent(currentTurn));
            game = this.ProcessEvents(game);

            return game;
        }

        /// <summary>
        /// Processes a single Turn, including all <see cref="Player"/> actions and any triggered effects.
        /// </summary>
        /// <param name="game">The <see cref="GameState"/> at the end of the previous Turn.</param>
        /// <returns>The <see cref="GameState"/> at the end of the new Turn.</returns>
        public GameState PlaySingleTurn(GameState game)
        {
            var lastTurn = game.Turn;

            // Don't continue if the game is over.
            // TODO: Consider throwing an error
            if (lastTurn >= 6)
            {
                // TODO: Allow for abilities that alter the number of turns
                return game;
            }

            game = this.StartNextTurn(game);

            // Get which player to resolve first
            var firstPlayerToResolve = game.GetLeader() ?? Random.Side();

            // Get player actions
            var topPlayerActions = game.Top.Controller.GetActions(game, firstPlayerToResolve);
            var bottomPlayerActions = game.Bottom.Controller.GetActions(game, firstPlayerToResolve);

            // Resolve player actions
            game = this.ProcessPlayerActions(game, topPlayerActions, bottomPlayerActions);

            // Reveal cards
            game = this.RevealCards(game, firstPlayerToResolve);


            this.logger.LogGameState(game);

            return game;
        }

        GameState RevealCards(GameState game, Side firstPlayerToResolve)
        {
            // TODO: Reveal played cards
            return game;
        }

        GameState ProcessPlayerActions(
            GameState game,
            IReadOnlyList<IPlayerAction> topPlayerActions,
            IReadOnlyList<IPlayerAction> bottomPlayerActions)
        {
            // Sanity check - ensure that the Actions are for the correct Player
            ValidatePlayerActions(topPlayerActions, Side.Top);
            ValidatePlayerActions(bottomPlayerActions, Side.Bottom);

            // TODO: Apply any constraints to actions (such as, cannot play cards at a given space)

            // TODO: Figure out how Nightcrawler is resolved when moving,
            // and whether there are any similar exceptions
            foreach (var action in topPlayerActions)
            {
                game = action.Apply(game);
            }

            foreach (var action in bottomPlayerActions)
            {
                game = action.Apply(game);
            }

            return game;
        }

        void ValidatePlayerActions(IReadOnlyList<IPlayerAction> actions, Side side)
        {
            if (actions.Any(a => a.Side != side))
            {
                var invalidAction = actions.First(a => a.Side != side);
                throw new InvalidOperationException($"{side} player action specified a Side of {invalidAction.Side}");
            }
        }

        GameState ProcessEvents(GameState gameState)
        {
            while (gameState.NewEvents.Count > 0)
            {
                // TODO: Actually do something useful for events
                var nextEvent = gameState.NewEvents[0];
                var remainingEvents = gameState.NewEvents.Skip(1).ToImmutableList();

                this.logger.LogEvent(nextEvent);

                var oldEvents = gameState.PastEvents.Add(nextEvent);
                gameState = gameState with { PastEvents = oldEvents, NewEvents = remainingEvents };
            }

            return gameState;
        }
    }
}
