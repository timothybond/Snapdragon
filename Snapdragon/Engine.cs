using System.Collections.Immutable;
using Snapdragon.CardAbilities;
using Snapdragon.Events;

namespace Snapdragon
{
    public class Engine
    {
        private readonly IGameLogger logger;

        public Engine(IGameLogger logger)
        {
            this.logger = logger;
        }

        public GameState CreateGame(
            PlayerConfiguration topPlayer,
            PlayerConfiguration bottomPlayer,
            bool shuffle = true,
            Side? firstRevealed = null
        )
        {
            var firstRevealedOrRandom = firstRevealed ?? Random.Side();

            // TODO: Specify different Locations, with effects
            // TODO: Handle card abilities that put them in a specific draw order
            return new GameState(
                0,
                new Location("Left", Column.Left),
                new Location("Middle", Column.Middle),
                new Location("Right", Column.Right),
                topPlayer.ToState(Side.Top, shuffle).DrawCard().DrawCard().DrawCard(),
                bottomPlayer.ToState(Side.Bottom, shuffle).DrawCard().DrawCard().DrawCard(),
                firstRevealedOrRandom,
                [],
                []
            );
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
        /// Helper that reveals the <see cref="Location"/> for the given turn, assuming it's turn 1-3.
        ///
        /// For all other turns, just returns the input <see cref="GameState"/>.
        /// </summary>
        private GameState RevealLocation(GameState game)
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
        public GameState PlayGame(GameState game)
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

            // Get player actions
            var topPlayerActions = game.Top.Controller.GetActions(game, Side.Top);
            var bottomPlayerActions = game.Bottom.Controller.GetActions(game, Side.Bottom);

            // Resolve player actions
            game = this.ProcessPlayerActions(game, topPlayerActions, bottomPlayerActions);

            // Reveal cards
            game = this.RevealCards(game);

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

        GameState RevealCards(GameState game)
        {
            game = RevealCardsForOneSide(game, game.FirstRevealed);
            game = RevealCardsForOneSide(game, game.FirstRevealed.OtherSide());

            return game;
        }

        /// <summary>
        /// Helper function that reveals only one Player's cards. Called in order by <see cref="RevealCards"/>.
        /// </summary>
        private GameState RevealCardsForOneSide(GameState game, Side side)
        {
            // TODO: Handle anything that delays revealing cards
            // Note all instances of CardPlayedEvent in the previous phase
            // should be processed now, because we call ProcessEvent in ProcessPlayerActions first.
            var cardPlayOrder = game
                .PastEvents.Where(e => e.Type == EventType.CardPlayed)
                .Cast<CardPlayedEvent>()
                .Select(cpe => cpe.Card.Id)
                .ToList();

            // Cards are revealed in the order they were played
            var unrevealedCards = game
                .AllCards.Where(c => c.Side == side && c.State == CardState.PlayedButNotRevealed)
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
        private GameState RevealCard(GameState game, Card card)
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
                }
            );

            return ProcessEvents(game);
        }

        GameState ProcessPlayerActions(
            GameState game,
            IReadOnlyList<IPlayerAction> topPlayerActions,
            IReadOnlyList<IPlayerAction> bottomPlayerActions
        )
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
                throw new InvalidOperationException(
                    $"{side} player action specified a Side of {invalidAction.Side}"
                );
            }
        }

        /// <summary>
        /// Processes any <see cref="Event"/>s in the <see cref="GameState.NewEvents"/> list,
        /// moving them to the <see cref="GameState.PastEvents"/> list when finished.
        ///
        /// Also has the side effect of recalculating the current power of all <see cref="Card"/>s.
        /// </summary>
        /// <returns>The new state with the appropriate changes applied.</returns>
        private GameState ProcessEvents(GameState game)
        {
            while (game.NewEvents.Count > 0)
            {
                // TODO: Feed events to triggers
                var nextEvent = game.NewEvents[0];
                this.logger.LogEvent(nextEvent);

                game = game.ProcessNextEvent();
            }

            game = RecalculateOngoingEffects(game);

            return game;
        }

        private GameState RecalculateOngoingEffects(GameState gameState)
        {
            var ongoingCardAbilities = gameState.GetCardOngoingAbilities().ToList();

            var recalculatedCards = gameState.AllCards.Select(c =>
                c with
                {
                    PowerAdjustment = this.GetPowerAdjustment(c, ongoingCardAbilities, gameState)
                }
            );

            return gameState.WithCards(recalculatedCards);
        }

        private int? GetPowerAdjustment(
            Card card,
            IReadOnlyList<(IOngoingAbility<Card> Ability, Card Source)> ongoingCardAbilities,
            GameState game
        )
        {
            var any = false;
            var total = 0;

            foreach (var ongoing in ongoingCardAbilities)
            {
                if (ongoing.Ability is OngoingAdjustPower<Card> adjustPower)
                {
                    var adjustment = adjustPower.Apply(card, ongoing.Source, game);
                    if (adjustment.HasValue)
                    {
                        total += adjustment.Value;
                        any = true;
                    }
                }
            }

            return any ? total : null;
        }
    }
}
