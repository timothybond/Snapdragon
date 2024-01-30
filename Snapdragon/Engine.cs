﻿using Snapdragon.Events;

namespace Snapdragon
{
    public class Engine
    {
        private readonly IGameLogger logger;

        public Engine(IGameLogger logger)
        {
            this.logger = logger;
        }

        public Game CreateGame(
            PlayerConfiguration topPlayer,
            PlayerConfiguration bottomPlayer,
            bool shuffle = true,
            Side? firstRevealed = null)
        {
            var firstRevealedOrRandom = firstRevealed ?? Random.Side();

            // TODO: Specify different Locations, with effects
            // TODO: Handle card abilities that put them in a specific draw order
            return new Game(
                0,
                new Location("Left", Column.Left),
                new Location("Middle", Column.Middle),
                new Location("Right", Column.Right),
                topPlayer.ToPlayer(Side.Top, shuffle).DrawCard().DrawCard().DrawCard(),
                bottomPlayer.ToPlayer(Side.Bottom, shuffle).DrawCard().DrawCard().DrawCard(),
                firstRevealedOrRandom,
                [],
                []);
        }

        /// <summary>
        /// Processes the beginning of a Turn.  Used inside <see cref="PlaySingleTurn(Game)"/>, but exposed here for
        /// unit-testing purposes.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> at the end of the previous Turn.</param>
        /// <returns>
        /// The <see cref="Game"/> at the start of the new Turn, before any <see cref="PlayerConfiguration"/> actions.
        /// </returns>
        public Game StartNextTurn(Game game)
        {
            // Note the check for Games going over is in PlaySingleTurn
            game = game with
            {
                Turn = game.Turn + 1
            };
            game = RevealLocation(game);
            game = ProcessEvents(game);

            // Each Player draws a card, and gets an amount of energy equal to the turn count
            var topPlayer = game.Top.DrawCard() with
            {
                Energy = game.Turn
            };
            var bottomPlayer = game.Bottom.DrawCard() with { Energy = game.Turn };

            game = game with { Top = topPlayer, Bottom = bottomPlayer };

            // Raise an event for the start of the turn
            game = game.WithEvent(new TurnStartedEvent(game.Turn));
            game = this.ProcessEvents(game);

            return game;
        }

        /// <summary>
        /// Helper that reveals the <see cref="Location"/> for the given turn, assuming it's turn 1-3.  For all other
        /// turns, just returns the input <see cref="Game"/>.
        /// </summary>
        private Game RevealLocation(Game game)
        {
            // TODO: Handle any effects that alter the reveal (are there any?)
            switch (game.Turn)
            {
                case 1:
                    return game.WithRevealedLocation(Column.Left);
                case 2:
                    return game.WithRevealedLocation(Column.Middle);
                case 3:
                    return game.WithRevealedLocation(Column.Right);
                default:
                    return game;
            }
        }

        /// <summary>
        /// Plays the game until it finishes.
        /// </summary>
        public Game PlayGame(Game game)
        {
            while (!game.GameOver)
            {
                game = PlaySingleTurn(game);
            }

            return game;
        }

        /// <summary>
        /// Processes a single Turn, including all <see cref="Player"/> actions and any triggered effects.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> at the end of the previous Turn.</param>
        /// <returns>The <see cref="Game"/> at the end of the new Turn.</returns>
        public Game PlaySingleTurn(Game game)
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

            // Get player actions
            var topPlayerActions = game.Top.Controller.GetActions(game, Side.Top);
            var bottomPlayerActions = game.Bottom.Controller.GetActions(game, Side.Bottom);

            // Resolve player actions
            game = this.ProcessPlayerActions(game, topPlayerActions, bottomPlayerActions);

            // Reveal cards
            game = this.RevealCards(game);

            game = game.EndTurn();
            game = this.ProcessEvents(game);

            this.logger.LogGameState(game);

            // TODO: Allow for abilities that alter the number of turns
            if (game.Turn >= 6)
            {
                game = game with { GameOver = true };
            }

            // Get which player to resolve first next turn
            var firstRevealed = game.GetLeader() ?? Random.Side();
            return game with { FirstRevealed = firstRevealed };
        }

        Game RevealCards(Game game)
        {
            game = RevealCardsForOneSide(game, game.FirstRevealed);
            game = RevealCardsForOneSide(game, game.FirstRevealed.OtherSide());

            return game;
        }

        /// <summary>
        /// Helper function that reveals only one Player's cards. Called in order by <see cref="RevealCards"/>.
        /// </summary>
        private Game RevealCardsForOneSide(Game game, Side side)
        {
            // TODO: Handle anything that delays revealing cards
            // Note all instances of CardPlayedEvent in the previous phase
            // should be processed now, because we call ProcessEvent in ProcessPlayerActions first.
            var cardPlayOrder = game
                .PastEvents
                .Where(e => e.Type == EventType.CardPlayed)
                .Cast<CardPlayedEvent>()
                .Select(cpe => cpe.Card.Id)
                .ToList();

            // Cards are revealed in the order they were played
            var unrevealedCards = game
                .AllCardsIncludingUnrevealed
                .Where(c => c.Side == side && c.State == CardState.PlayedButNotRevealed)
                .OrderBy(c => cardPlayOrder.IndexOf(c.Id));

            foreach (var card in unrevealedCards)
            {
                game = RevealCard(game, card);
            }

            return game;
        }

        /// <summary>
        /// Helper function that reveals a single card, then processes any triggered events.
        /// </summary>
        private Game RevealCard(Game game, Card card)
        {
            game = game.WithModifiedCard(
                card,
                c => c with { State = CardState.InPlay },
                (g, c) =>
                {
                    if (c.OnReveal != null)
                    {
                        g = c.OnReveal.Activate(g, c);
                    }

                    return g.WithEvent(new CardRevealedEvent(g.Turn, c));
                });

            return ProcessEvents(game);
        }

        Game ProcessPlayerActions(
            Game game,
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

            return ProcessEvents(game);
        }

        void ValidatePlayerActions(IReadOnlyList<IPlayerAction> actions, Side side)
        {
            if (actions.Any(a => a.Side != side))
            {
                var invalidAction = actions.First(a => a.Side != side);
                throw new InvalidOperationException($"{side} player action specified a Side of {invalidAction.Side}");
            }
        }

        /// <summary>
        /// Processes any <see cref="Event"/>s in the <see cref="Game.NewEvents"/> list, moving them to the <see
        /// cref="Game.PastEvents"/> list when finished.  Also has the side effect of recalculating the current power of
        /// all <see cref="Card"/>s.
        /// </summary>
        /// <returns>The new state with the appropriate changes applied.</returns>
        private Game ProcessEvents(Game game)
        {
            while (game.NewEvents.Count > 0)
            {
                // TODO: Feed events to triggers
                var nextEvent = game.NewEvents[0];
                this.logger.LogEvent(nextEvent);

                game = game.ProcessNextEvent();
            }

            game = game.RecalculateOngoingEffects();

            return game;
        }
    }
}
