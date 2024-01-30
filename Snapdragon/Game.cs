using System.Collections.Immutable;
using Snapdragon.Events;
using Snapdragon.OngoingAbilities;

namespace Snapdragon
{
    public record Game(
        int Turn,
        Location Left,
        Location Middle,
        Location Right,
        Player Top,
        Player Bottom,
        Side FirstRevealed,
        ImmutableList<Event> PastEvents,
        ImmutableList<Event> NewEvents,
        IGameLogger Logger,
        bool GameOver = false
    )
    {
        #region Accessors

        /// <summary>
        /// Gets the <see cref="Location"/> in the given <see cref="Column"/>.
        public Location this[Column column]
        {
            get
            {
                switch (column)
                {
                    case Column.Left:
                        return Left;
                    case Column.Middle:
                        return Middle;
                    case Column.Right:
                        return Right;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Player"/> on the given <see cref="Side"/>.
        /// </summary>
        public Player this[Side side]
        {
            get
            {
                switch (side)
                {
                    case Side.Top:
                        return Top;
                    case Side.Bottom:
                        return Bottom;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets all <see cref="Card"/>s that have been played and revealed.
        /// </summary>
        public IEnumerable<Card> AllCards =>
            this
                .Left.AllCards.Concat(this.Middle.AllCards)
                .Concat(this.Right.AllCards)
                .Where(c => c.State == CardState.InPlay);

        /// <summary>
        /// Gets all <see cref="Card"/>s that have been played, whether or not they are revealed.
        /// </summary>
        public IEnumerable<Card> AllCardsIncludingUnrevealed =>
            this.Left.AllCards.Concat(this.Middle.AllCards).Concat(this.Right.AllCards);

        public IEnumerable<TemporaryEffect<Card>> AllCardTemporaryEffects =>
            this
                .Left.TemporaryCardEffects.Concat(this.Middle.TemporaryCardEffects)
                .Concat(this.Right.TemporaryCardEffects);

        public IEnumerable<(IOngoingAbility<Card> Ability, Card Source)> GetCardOngoingAbilities()
        {
            foreach (var column in All.Columns)
            {
                foreach (var side in All.Sides)
                {
                    foreach (var card in this[column][side])
                    {
                        if (card.Ongoing != null)
                        {
                            yield return (card.Ongoing, card);
                        }
                    }
                }
            }
        }

        #endregion

        #region Direct Manipulation

        /// <summary>
        /// Gets a modified state with the specified new <see cref="Event"/> added.
        /// </summary>
        public Game WithEvent(Event e)
        {
            return this with { NewEvents = this.NewEvents.Add(e) };
        }

        /// <summary>
        /// Gets a modified state that includes the passed-in <see cref="Player"/> as appropriate.
        /// </summary>
        public Game WithPlayer(Player player)
        {
            switch (player.Side)
            {
                case Side.Top:
                    return this with { Top = player };
                case Side.Bottom:
                    return this with { Bottom = player };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a modified state that includes the passed-in <see cref="Location"/> as appropriate.
        /// </summary>
        public Game WithLocation(Location location)
        {
            switch (location.Column)
            {
                case Column.Left:
                    return this with { Left = location };
                case Column.Middle:
                    return this with { Middle = location };
                case Column.Right:
                    return this with { Right = location };
                default:
                    throw new NotImplementedException();
            }
        }

        public Game WithRevealedLocation(Column column)
        {
            var location = this[column] with { Revealed = true };

            return this.WithLocation(location)
                .WithEvent(new LocationRevealedEvent(this.Turn, location));
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="TemporaryEffect{Card}"/>.  Note that unlike <see
        /// cref="WithCard(Card)"/>, this adds a new effect rather than modifying an existing one.
        /// </summary>
        public Game WithTemporaryCardEffect(TemporaryEffect<Card> temporaryCardEffect)
        {
            var location = this[temporaryCardEffect.Column];

            return this.WithLocation(location.WithTemporaryCardEffect(temporaryCardEffect));
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="TemporaryEffect{Card}"/>.  Note that unlike <see
        /// cref="WithCard(Card)"/>, this adds a new effect rather than modifying an existing one.
        /// </summary>
        public Game WithTemporaryCardEffectDeleted(int temporaryCardEffectId)
        {
            return this with
            {
                Left = this.Left.WithTemporaryCardEffectDeleted(temporaryCardEffectId),
                Middle = this.Middle.WithTemporaryCardEffectDeleted(temporaryCardEffectId),
                Right = this.Right.WithTemporaryCardEffectDeleted(temporaryCardEffectId),
            };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Card"/>s updated.  Currently only suitable for cards in
        /// play, with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithCards(IEnumerable<Card> cards)
        {
            // TODO: Determine if this needs to be optimized
            var game = this;

            foreach (var card in cards)
            {
                game = game.WithCard(card);
            }

            return game;
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Card"/> updated.  Currently only suitable for cards in play,
        /// with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithCard(Card card)
        {
            var column =
                card.Column
                ?? throw new InvalidOperationException(
                    "Tried to modify a card that isn't in play."
                );

            var location = this[column];
            var newCards = location[card.Side]
                .Select(c => c.Id == card.Id ? card : c)
                .ToImmutableList();

            location = location with
            {
                TopPlayerCards = card.Side == Side.Top ? newCards : location.TopPlayerCards,
                BottomPlayerCards =
                    card.Side == Side.Bottom ? newCards : location.BottomPlayerCards,
            };

            return this.WithLocation(location);
        }

        /// <summary>
        /// Gets a modified state that applies some change to a <see cref="Card"/> (in place).  Moves or side changes
        /// need to be handled elsewhere.
        /// </summary>
        /// <param name="currentCard">The existing card to be modified.</param>
        /// <param name="modifier">The modification to perform on the existing card.</param>
        /// <param name="postModifyTransform">
        /// Any change to the <see cref="Game"/> to follow the modification (typically, this will be used to raise
        /// events, like <see cref="CardRevealedEvent"/>).
        /// </param>
        public Game WithModifiedCard(
            Card currentCard,
            Func<Card, Card> modifier,
            Func<Game, Card, Game>? postModifyTransform = null
        )
        {
            if (currentCard.Column == null) { }

            Column column =
                currentCard.Column
                ?? throw new InvalidOperationException(
                    "Tried to modify a card that isn't in play."
                );

            var location = this[column];
            var side = currentCard.Side;

            var currentCardsForSide = this[column][side];

            // Cards still need to be placed in the same order (I think)
            var newCardsForSide = new List<Card>();

            var newCard = modifier(currentCard);

            for (var i = 0; i < currentCardsForSide.Count; i++)
            {
                if (currentCardsForSide[i].Id == currentCard.Id)
                {
                    newCardsForSide.Add(newCard);
                }
                else
                {
                    newCardsForSide.Add(currentCardsForSide[i]);
                }
            }

            switch (newCard.Side)
            {
                case Side.Top:
                    location = location with { TopPlayerCards = newCardsForSide.ToImmutableList() };
                    break;
                case Side.Bottom:
                    location = location with
                    {
                        BottomPlayerCards = newCardsForSide.ToImmutableList()
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            var newGame = this.WithLocation(location);

            if (postModifyTransform != null)
            {
                newGame = postModifyTransform(newGame, newCard);
            }

            return newGame;
        }

        #endregion

        #region Game Progression Logic

        /// <summary>
        /// Plays the game until it finishes.
        /// </summary>
        public Game PlayGame()
        {
            var game = this;

            while (!game.GameOver)
            {
                game = game.PlaySingleTurn();
            }

            return game;
        }

        /// <summary>
        /// Processes a single Turn, including all <see cref="Player"/> actions and any triggered effects.
        /// </summary>
        /// <returns>The <see cref="Game"/> at the end of the new Turn.</returns>
        public Game PlaySingleTurn()
        {
            var lastTurn = this.Turn;

            // Don't continue if the game is over.
            // TODO: Consider throwing an error
            if (lastTurn >= 6)
            {
                // TODO: Allow for abilities that alter the number of turns
                return this;
            }

            var game = this.StartNextTurn();

            // Get player actions
            var topPlayerActions = game.Top.Controller.GetActions(game, Side.Top);
            var bottomPlayerActions = game.Bottom.Controller.GetActions(game, Side.Bottom);

            // Resolve player actions
            game = game.ProcessPlayerActions(topPlayerActions, bottomPlayerActions);

            // Reveal cards
            game = this.RevealCards(game);

            game = game.EndTurn();
            game = game.ProcessEvents();

            this.Logger.LogGameState(game);

            // TODO: Allow for abilities that alter the number of turns
            if (game.Turn >= 6)
            {
                game = game with { GameOver = true };
            }

            // Get which player to resolve first next turn
            var firstRevealed = game.GetLeader() ?? Random.Side();
            return game with { FirstRevealed = firstRevealed };
        }

        Game ProcessPlayerActions(
            IReadOnlyList<IPlayerAction> topPlayerActions,
            IReadOnlyList<IPlayerAction> bottomPlayerActions
        )
        {
            // Sanity check - ensure that the Actions are for the correct Player
            ValidatePlayerActions(topPlayerActions, Side.Top);
            ValidatePlayerActions(bottomPlayerActions, Side.Bottom);

            // TODO: Apply any constraints to actions (such as, cannot play cards at a given space)
            var game = this;

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

            return game.ProcessEvents();
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
                .PastEvents.Where(e => e.Type == EventType.CardPlayed)
                .Cast<CardPlayedEvent>()
                .Select(cpe => cpe.Card.Id)
                .ToList();

            // Cards are revealed in the order they were played
            var unrevealedCards = game
                .AllCardsIncludingUnrevealed.Where(c =>
                    c.Side == side && c.State == CardState.PlayedButNotRevealed
                )
                .OrderBy(c => cardPlayOrder.IndexOf(c.Id));

            foreach (var card in unrevealedCards)
            {
                game = game.RevealCard(card);
            }

            return game;
        }

        /// <summary>
        /// Helper function that reveals a single card, then processes any triggered events.
        /// </summary>
        private Game RevealCard(Card card)
        {
            var game = this.WithModifiedCard(
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

            return game.ProcessEvents();
        }

        /// <summary>
        /// Processes the beginning of a Turn.  Used inside <see cref="PlaySingleTurn()"/>, but exposed here for
        /// unit-testing purposes.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> at the end of the previous Turn.</param>
        /// <returns>
        /// The <see cref="Game"/> at the start of the new Turn, before any <see cref="PlayerConfiguration"/> actions.
        /// </returns>
        public Game StartNextTurn()
        {
            // Note the check for Games going over is in PlaySingleTurn
            var game = this with
            {
                Turn = this.Turn + 1
            };
            game = game.RevealLocation();
            game = game.ProcessEvents();

            // Each Player draws a card, and gets an amount of energy equal to the turn count
            var topPlayer = game.Top.DrawCard() with
            {
                Energy = game.Turn
            };
            var bottomPlayer = game.Bottom.DrawCard() with { Energy = game.Turn };

            game = game with { Top = topPlayer, Bottom = bottomPlayer };

            // Raise an event for the start of the turn
            game = game.WithEvent(new TurnStartedEvent(game.Turn));
            game = game.ProcessEvents();

            return game;
        }

        /// <summary>
        /// Helper that reveals the <see cref="Location"/> for the given turn, assuming it's turn 1-3.  For all other
        /// turns, just returns the input <see cref="Game"/>.
        /// </summary>
        private Game RevealLocation()
        {
            // TODO: Handle any effects that alter the reveal (are there any?)
            switch (this.Turn)
            {
                case 1:
                    return this.WithRevealedLocation(Column.Left);
                case 2:
                    return this.WithRevealedLocation(Column.Middle);
                case 3:
                    return this.WithRevealedLocation(Column.Right);
                default:
                    return this;
            }
        }

        /// <summary>
        /// Gets the modified state after ending the current turn and processing any raised events.
        /// </summary>
        /// <returns></returns>
        public Game EndTurn()
        {
            return this.WithEvent(new TurnEndedEvent(this.Turn));
        }

        /// <summary>
        /// Processes any <see cref="Event"/>s in the <see cref="Game.NewEvents"/> list, moving them to the <see
        /// cref="Game.PastEvents"/> list when finished. Also has the side effect of recalculating the current power of
        /// all <see cref="Card"/>s.
        /// </summary>
        /// <returns>The new state with the appropriate changes applied.</returns>
        public Game ProcessEvents()
        {
            var game = this;

            while (game.NewEvents.Count > 0)
            {
                game = game.ProcessNextEvent();
            }

            game = game.RecalculateOngoingEffects();

            return game;
        }

        private Game ProcessNextEvent()
        {
            if (NewEvents.Count == 0)
            {
                return this;
            }

            var nextEvent = NewEvents[0];

            this.Logger.LogEvent(nextEvent);

            var remainingEvents = NewEvents.Skip(1).ToImmutableList();

            var oldEvents = PastEvents.Add(nextEvent);

            var game = this with { PastEvents = oldEvents, NewEvents = remainingEvents };

            // TODO: Determine if we need to stack-order events for triggers
            foreach (var cardWithTrigger in AllCards.Where(c => c.Triggered != null))
            {
                game = cardWithTrigger.Triggered?.ProcessEvent(game, nextEvent) ?? game;
            }

            foreach (var temporaryCardEffect in AllCardTemporaryEffects)
            {
                game = temporaryCardEffect.Ability?.ProcessEvent(game, nextEvent) ?? game;
            }

            return game;
        }

        public Game RecalculateOngoingEffects()
        {
            var ongoingCardAbilities = this.GetCardOngoingAbilities().ToList();

            var recalculatedCards = this.AllCards.Select(c =>
                c with
                {
                    PowerAdjustment = this.GetPowerAdjustment(c, ongoingCardAbilities, this)
                }
            );

            return this.WithCards(recalculatedCards);
        }

        /// <summary>
        /// Calculates the total power adjustment to the given <see cref="Card"/>
        /// based on the pased-in list of all active ongoing abilities
        /// </summary>
        private int? GetPowerAdjustment(
            Card card,
            IReadOnlyList<(IOngoingAbility<Card> Ability, Card Source)> ongoingCardAbilities,
            Game game
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

        #endregion

        #region Scoring

        public CurrentScores GetCurrentScores()
        {
            var scores = new CurrentScores();

            foreach (var column in All.Columns)
            {
                var location = this[column];

                // First sum the adjusted power of all cards
                foreach (var side in All.Sides)
                {
                    var totalPower = location[side].Sum(c => c.AdjustedPower);
                    scores = scores.WithAddedPower(totalPower, column, side);
                }

                // Now apply any ongoing effects that ADD power to a location (e.g. Mister Fantastic, Klaw)
                foreach (var card in this.AllCards)
                {
                    if (card.Ongoing is OngoingAddLocationPower<Card> addLocationPower)
                    {
                        if (addLocationPower.LocationFilter.Applies(location, card, this))
                        {
                            // TODO: Deal with the fact that the card isn't the "target"
                            var power = addLocationPower.Amount.GetValue(this, card, card);

                            // TODO: Check if anything adds power to the opposite side (probably the case)
                            scores = scores.WithAddedPower(power, column, card.Side);
                        }
                    }
                }

                // Now handle the special "double power" ability
                // TODO: Determine how this sometimes stacks
                foreach (var side in All.Sides)
                {
                    foreach (var card in location[side])
                    {
                        // For now I'm just ignoring the multiple doublings
                        if (card.Ongoing is DoubleLocationPower)
                        {
                            scores = scores.WithAddedPower(scores[column][side], column, side);
                            break;
                        }
                    }
                }
            }

            return scores;
        }

        /// <summary>
        /// Get the <see cref="Side"/> of the <see cref="Player"/> who is currently winning, meaning they have control
        /// of more <see cref="Locations"/> or, in the event of a tie, they have more Power overall.
        /// </summary>
        /// <returns>
        /// The <see cref="Side"/> of the <see cref="Player"/> currently in the lead, or <c>null</c> if they are tied in
        /// both <see cref="Location"/>s and Power.
        /// </returns>
        public Side? GetLeader()
        {
            var scores = this.GetCurrentScores();

            return scores.Leader;
        }

        #endregion
    }
}
