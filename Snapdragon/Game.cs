﻿using Snapdragon.Events;
using Snapdragon.OngoingAbilities;
using System.Collections.Immutable;

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

        public IEnumerable<Location> Locations
        {
            get
            {
                yield return Left;
                yield return Middle;
                yield return Right;
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

        public IEnumerable<Sensor<Card>> AllSensors =>
            this.Left.Sensors.Concat(this.Middle.Sensors).Concat(this.Right.Sensors);

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

        public IReadOnlySet<EffectType> GetBlockedEffects(Column column)
        {
            var set = new HashSet<EffectType>();
            var location = this[column];

            foreach (var source in AllCards)
            {
                if (source.Ongoing is OngoingBlockLocationEffect<Card> blockLocationEffect)
                {
                    if (blockLocationEffect.Applies(location, source, this))
                    {
                        set.Add(blockLocationEffect.EffectType);
                    }
                }
            }

            return set;
        }

        public IReadOnlySet<EffectType> GetBlockedEffects(Card card)
        {
            var set = new HashSet<EffectType>();

            if (card.Column.HasValue)
            {
                // This is a little gross but it's co-located with the method we're abusing
                set = (HashSet<EffectType>)GetBlockedEffects(card.Column.Value);
            }

            foreach (var source in AllCards)
            {
                if (source.Ongoing is OngoingBlockCardEffect<Card> blockCardEffect)
                {
                    if (blockCardEffect.Applies(card, source, this))
                    {
                        set.Add(blockCardEffect.EffectType);
                    }
                }
            }

            if (card.Disallowed != null)
            {
                foreach (var selfDisallowed in card.Disallowed)
                {
                    set.Add(selfDisallowed);
                }
            }

            return set;
        }

        /// <summary>
        /// Determines if the given card can be moved to the specified column,
        /// based on whatever abilities it has or other cards have.
        ///
        /// This will also check for blocked movement effects,
        /// but will NOT check for how many columns are in use at the destination.
        /// </summary>
        public bool CanMove(Card card, Column destination)
        {
            // Note: This weird scope exists because I didn't feel like keeping around two references to the same thing,
            // but I couldn't directly replace "card" until I verified that it wasn't null.
            {
                var actualCard = AllCards.SingleOrDefault(c => c.Id == card.Id);

                if (actualCard == null)
                {
                    return false;
                }

                card = actualCard;
            }

            if (card.Column == null)
            {
                throw new InvalidOperationException("Somehow a card in play has no Column set.");
            }

            var blockedEffects = GetBlockedEffects(card);
            if (blockedEffects.Contains(EffectType.MoveCard))
            {
                return false;
            }

            var blockedAtFrom = GetBlockedEffects(card.Column.Value);
            if (blockedAtFrom.Contains(EffectType.MoveFromLocation))
            {
                return false;
            }

            var blockedAtTo = GetBlockedEffects(destination);
            if (blockedAtTo.Contains(EffectType.MoveToLocation))
            {
                return false;
            }

            foreach (var cardInPlay in AllCards)
            {
                if (
                    cardInPlay.MoveAbility?.CanMove(card, cardInPlay, destination, this)
                    ?? false
                )
                {
                    return true;
                }
            }

            foreach (var sensor in AllSensors)
            {
                if (sensor.MoveAbility?.CanMove(card, sensor, destination, this) ?? false)
                {
                    return true;
                }
            }

            return false;
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
        /// Gets a modified state with the given <see cref="Sensor{Card}"/>.  Note that unlike <see
        /// cref="WithCard(Card)"/>, this adds a new effect rather than modifying an existing one.
        /// </summary>
        public Game WithTemporaryCardEffect(Sensor<Card> temporaryCardEffect)
        {
            var location = this[temporaryCardEffect.Column];

            return this.WithLocation(location.WithSensor(temporaryCardEffect));
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{Card}"/>.  Note that unlike <see
        /// cref="WithCard(Card)"/>, this adds a new effect rather than modifying an existing one.
        /// </summary>
        public Game WithTemporaryCardEffectDeleted(int temporaryCardEffectId)
        {
            return this with
            {
                Left = this.Left.WithSensorDeleted(temporaryCardEffectId),
                Middle = this.Middle.WithSensorDeleted(temporaryCardEffectId),
                Right = this.Right.WithSensorDeleted(temporaryCardEffectId),
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
        /// Somewhat-weird method that plays a turn from the state after <see cref="StartNextTurn"/> is called.
        ///
        /// This normally will be called inside <see cref="PlaySingleTurn"/>, but is exposed because
        /// it's useful for any <see cref="IPlayerController"/> that might explore future pathways
        /// off of the current state, and therefore need to be able to play without duplicatively
        /// triggering the start-of-turn logic.
        /// </summary>
        /// <returns></returns>
        public Game PlayAlreadyStartedTurn()
        {
            var game = this;

            // Get player actions
            var topPlayerActions = game.Top.Controller.GetActions(game, Side.Top);
            var bottomPlayerActions = game.Bottom.Controller.GetActions(game, Side.Bottom);

            // Resolve player actions
            game = game.ProcessPlayerActions(topPlayerActions, bottomPlayerActions);

            // Reveal cards
            game = this.RevealCards(game);

            game = game.EndTurn();
            game = game.ProcessEvents();
            game = game.RecalculateOngoingEffects();

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

            game = game.PlayAlreadyStartedTurn();

            return game;
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
            game = RevealCardsForOneSide(game, game.FirstRevealed.Other());

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
                    g = g.WithEvent(new CardRevealedEvent(g.Turn, c));

                    if (c.OnReveal != null)
                    {
                        g = c.OnReveal.Activate(g, c);
                    }

                    return g;
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
        /// cref="Game.PastEvents"/> list when finished.
        /// </summary>
        /// <returns>The new state with the appropriate changes applied.</returns>
        public Game ProcessEvents()
        {
            var game = this;

            while (game.NewEvents.Count > 0)
            {
                game = game.ProcessNextEvent();
            }

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

            // Note: Becuase we modify the game, we need to capture the state of it before this effect triggers.
            // E.g., if we return a card to somebody's hand, we don't want to fire another trigger on it.
            var cardsInPlay = AllCards;
            var cardsInHands = Top.Hand.Concat(Bottom.Hand);
            var cardsInLibraries = Top.Library.Cards.Concat(Bottom.Library.Cards);
            var discardedAndDestroyed = Top
                .Discards.Concat(Bottom.Discards)
                .Concat(Top.Destroyed)
                .Concat(Bottom.Destroyed);
            var sensors = AllSensors;

            // TODO: Determine if we need to stack-order events for triggers
            foreach (var cardWithTrigger in cardsInPlay.Where(c => c.Triggered != null))
            {
                if (cardWithTrigger.State == CardState.InPlay)
                {
                    game =
                        cardWithTrigger.Triggered?.ProcessEvent(game, nextEvent, cardWithTrigger)
                        ?? game;
                }
            }

            foreach (
                var discardedOrDestroyedCard in discardedAndDestroyed.Where(c =>
                    c.Triggered?.DiscardedOrDestroyed == true
                )
            )
            {
                game =
                    discardedOrDestroyedCard.Triggered?.ProcessEvent(
                        game,
                        nextEvent,
                        discardedOrDestroyedCard
                    ) ?? game;
            }

            foreach (
                var cardInHand in cardsInHands.Where(c => c.Triggered?.DiscardedOrDestroyed == true)
            )
            {
                game = cardInHand.Triggered?.ProcessEvent(game, nextEvent, cardInHand) ?? game;
            }

            foreach (var cardInLibrary in cardsInLibraries.Where(c => c.Triggered?.InDeck == true))
            {
                game =
                    cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary) ?? game;
            }

            foreach (var sensor in sensors)
            {
                game = sensor.TriggeredAbility?.ProcessEvent(game, nextEvent, sensor) ?? game;
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
