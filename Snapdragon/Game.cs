using System.Collections.Immutable;
using Snapdragon.Events;
using Snapdragon.OngoingAbilities;

namespace Snapdragon
{
    public record Game(
        Guid Id,
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
        /// Gets all <see cref="CardInstance"/>s that have been played and revealed.
        /// </summary>
        public IEnumerable<Card> AllCards =>
            this
                .Left.AllCards.Concat(this.Middle.AllCards)
                .Concat(this.Right.AllCards)
                .Where(c => c.State == CardState.InPlay);

        /// <summary>
        /// Gets all <see cref="CardInstance"/>s that have been played, whether or not they are revealed.
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

        public IEnumerable<(
            IOngoingAbility<Location> Ability,
            Location Source
        )> GetLocationOngoingAbilities()
        {
            foreach (var location in Locations)
            {
                if (location.Revealed && location.Definition.Ongoing != null)
                {
                    yield return (location.Definition.Ongoing, location);
                }
            }
        }

        public IReadOnlySet<EffectType> GetBlockedEffects(
            Column column,
            Side side,
            IReadOnlyList<Card>? cardsWithLocationEffectBlocks = null,
            IReadOnlyList<Location>? locationsWithLocationEffectBlocks = null
        )
        {
            var set = new HashSet<EffectType>();
            var location = this[column];

            foreach (var source in cardsWithLocationEffectBlocks ?? AllCards)
            {
                if (source.Ongoing is OngoingBlockLocationEffect<Card> blockLocationEffect)
                {
                    if (blockLocationEffect.Applies(location, side, source, this))
                    {
                        set.Add(blockLocationEffect.EffectType);
                    }
                }
            }

            foreach (var loc in locationsWithLocationEffectBlocks ?? Locations)
            {
                if (
                    loc.Revealed
                    && loc.Definition.Ongoing
                        is OngoingBlockLocationEffect<Location> blockLocationEffect
                )
                {
                    if (blockLocationEffect.Applies(location, side, loc, this))
                    {
                        set.Add(blockLocationEffect.EffectType);
                    }
                }
            }

            return set;
        }

        /// <summary>
        /// Gets the types of effects that are blocked for the given card,
        /// including those blocked based on its current location.
        ///
        /// Optional parameters are passed in for optimization (primarily for ControllerUtilities).
        /// </summary>
        public IReadOnlySet<EffectType> GetBlockedEffects(
            Card card,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>>? blockedEffectsByColumn = null,
            IReadOnlyList<Card>? cardsWithCardEffectBlocks = null
        )
        {
            var set = new HashSet<EffectType>();

            if (blockedEffectsByColumn != null)
            {
                foreach (var blockedEffect in blockedEffectsByColumn[card.Column])
                {
                    set.Add(blockedEffect);
                }
            }
            else
            {
                // This is a little gross but it's co-located with the method we're abusing
                set = (HashSet<EffectType>)GetBlockedEffects(card.Column, card.Side);
            }

            foreach (var source in cardsWithCardEffectBlocks ?? AllCards)
            {
                if (source.Ongoing is OngoingBlockCardEffect<ICard> blockCardEffect)
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
        /// That's important in how it's used, particularly in ControllerUtilities.
        ///
        /// As a performance optimization, the caller can pass in blocked effects by location,
        /// which - WARNING - MUST already be for the correct side.
        /// </summary>
        public bool CanMove(
            Card card,
            Column destination,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>>? blockedEffectsByColumn = null,
            IReadOnlyList<Card>? cardsWithMoveAbilities = null,
            IReadOnlyList<Sensor<Card>>? sensorsWithMoveAbilities = null,
            IReadOnlyList<Card>? cardsWithCardEffectBlocks = null
        )
        {
            // Note: This weird scope exists because I didn't feel like keeping around two references to the same thing,
            // but I couldn't directly replace "card" until I verified that it wasn't null.
            {
                var actualCard = this[card.Column][card.Side].SingleOrDefault(c => c.Id == card.Id);

                if (actualCard == null)
                {
                    return false;
                }

                card = actualCard;
            }

            var blockedEffects = GetBlockedEffects(
                card,
                blockedEffectsByColumn,
                cardsWithCardEffectBlocks
            );
            if (blockedEffects.Contains(EffectType.MoveCard))
            {
                return false;
            }

            var blockedAtFrom =
                blockedEffectsByColumn?[card.Column] ?? GetBlockedEffects(card.Column, card.Side);
            if (blockedAtFrom.Contains(EffectType.MoveFromLocation))
            {
                return false;
            }

            var blockedAtTo =
                blockedEffectsByColumn?[card.Column] ?? GetBlockedEffects(destination, card.Side);
            if (blockedAtTo.Contains(EffectType.MoveToLocation))
            {
                return false;
            }

            foreach (var cardInPlay in cardsWithMoveAbilities ?? AllCards)
            {
                if (cardInPlay.MoveAbility?.CanMove(card, cardInPlay, destination, this) ?? false)
                {
                    return true;
                }
            }

            foreach (var sensor in sensorsWithMoveAbilities ?? AllSensors)
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

        public Game DrawCard(Side side)
        {
            var player = this[side];

            // TODO: Check for any blocks on drawing cards
            if (player.Library.Count > 0 && player.Hand.Count < Max.HandSize)
            {
                player = player.DrawCard();
                return this.WithPlayer(player)
                    .WithEvent(new CardDrawnEvent(Turn, player.Hand.Last()));
            }

            return this;
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

            var game = this.WithLocation(location)
                .WithEvent(new LocationRevealedEvent(this.Turn, location));

            if (location.Definition.OnReveal != null)
            {
                game = location.Definition.OnReveal.Activate(game, location);
            }

            return game;
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{Card}"/>.  Note that unlike <see
        /// cref="WithCard(CardInstance)"/>, this adds a new Sensor rather than modifying an existing one.
        /// </summary>
        public Game WithSensor(Sensor<Card> sensor)
        {
            var location = this[sensor.Column];

            return this.WithLocation(location.WithSensor(sensor));
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{Card}"/>.  Note that unlike <see
        /// cref="WithCard(CardInstance)"/>, this adds a new Sensor rather than modifying an existing one.
        /// </summary>
        public Game WithSensorDeleted(long sensorId)
        {
            return this with
            {
                Left = this.Left.WithSensorDeleted(sensorId),
                Middle = this.Middle.WithSensorDeleted(sensorId),
                Right = this.Right.WithSensorDeleted(sensorId),
            };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="CardInstance"/>s updated.  Currently only suitable for cards in
        /// play, with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithUpdatedCards(IEnumerable<Card> cards)
        {
            // TODO: Determine if this needs to be optimized
            var game = this;

            foreach (var card in cards)
            {
                game = game.WithUpdatedCard(card);
            }

            return game;
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="CardInstance"/> updated.  Currently only suitable for cards in play,
        /// with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithUpdatedCard(Card card)
        {
            var location = this[card.Column];
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
        /// Gets a modified state that applies some change to a <see cref="CardInstance"/> (in place).  Moves or side changes
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
            var location = this[currentCard.Column];
            var side = currentCard.Side;

            var currentCardsForSide = this[currentCard.Column][side];

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
            try
            {
                var game = this;

                while (!game.GameOver)
                {
                    game = game.PlaySingleTurn();
                }

                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Aborting game due to error:\n\n{ex}\n\nEnding game state:\n\n{LoggerUtilities.GameStateLog(this)}"
                );
                throw;
            }
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
                c => c with { State = CardState.InPlay, TurnRevealed = this.Turn },
                (g, c) =>
                {
                    if (c.OnReveal != null)
                    {
                        g = c.OnReveal.Activate(g, c);

                        // This is to ensure that cards that get modified by their own reveal
                        // abilities get attached to the reveal event in their modified state.
                        //
                        // I'm not totally sold on whether this is the right solution.
                        // In some cases we proactively re-fetch the card by ID when performing
                        // effects, but that isn't always true (hence me making this change).
                        //
                        // Also, the null coalesce operator is because Hulkbuster.
                        c = g.AllCards.SingleOrDefault(x => x.Id == c.Id) ?? c;
                    }
                    g = g.WithEvent(new CardRevealedEvent(g.Turn, c));

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
            game = game.DrawCard(Side.Top).DrawCard(Side.Bottom);
            var topPlayer = game.Top with { Energy = game.Turn };
            var bottomPlayer = game.Bottom with { Energy = game.Turn };

            game = game with { Top = topPlayer, Bottom = bottomPlayer };

            Logger.LogHands(game);

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
            var originalState = game;

            var sensors = AllSensors;

            // TODO: Determine if we need to stack-order events for triggers, any other ordering constraints

            // All this stuff is unrolled and not using helper accessors in an effort to avoid allocations / boost performance
            for (var i = 0; i < originalState.Left.TopPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Left.TopPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Left.BottomPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Left.BottomPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Middle.TopPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Middle.TopPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Middle.BottomPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Middle.BottomPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Right.TopPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Right.TopPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Right.BottomPlayerCards.Count; i++)
            {
                var cardWithTrigger = originalState.Right.BottomPlayerCards[i];
                if (cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            for (var i = 0; i < originalState.Top.Discards.Count; i++)
            {
                var discardedOrDestroyedCard = originalState.Top.Discards[i];
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed)
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            for (var i = 0; i < originalState.Top.Destroyed.Count; i++)
            {
                var discardedOrDestroyedCard = originalState.Top.Destroyed[i];
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed)
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            for (var i = 0; i < originalState.Bottom.Discards.Count; i++)
            {
                var discardedOrDestroyedCard = originalState.Bottom.Discards[i];
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed)
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            for (var i = 0; i < originalState.Bottom.Destroyed.Count; i++)
            {
                var discardedOrDestroyedCard = originalState.Bottom.Destroyed[i];
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed)
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            for (var i = 0; i < originalState.Top.Hand.Count; i++)
            {
                var cardInHand = originalState.Top.Hand[i];
                if (cardInHand.Triggered != null)
                {
                    if (cardInHand.Triggered.InHand)
                    {
                        game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
                    }
                }
            }

            for (var i = 0; i < originalState.Bottom.Hand.Count; i++)
            {
                var cardInHand = originalState.Bottom.Hand[i];
                if (cardInHand.Triggered != null)
                {
                    if (cardInHand.Triggered.InHand)
                    {
                        game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
                    }
                }
            }

            for (var i = 0; i < originalState.Top.Library.Cards.Count; i++)
            {
                var cardInLibrary = originalState.Top.Library.Cards[i];
                if (cardInLibrary.Triggered != null)
                {
                    if (cardInLibrary.Triggered.InDeck)
                    {
                        game =
                            cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary)
                            ?? game;
                    }
                }
            }

            for (var i = 0; i < originalState.Bottom.Library.Cards.Count; i++)
            {
                var cardInLibrary = originalState.Bottom.Library.Cards[i];
                if (cardInLibrary.Triggered != null)
                {
                    if (cardInLibrary.Triggered.InDeck)
                    {
                        game =
                            cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary)
                            ?? game;
                    }
                }
            }

            foreach (var sensor in sensors)
            {
                game = sensor.TriggeredAbility?.ProcessEvent(game, nextEvent, sensor) ?? game;
            }

            foreach (var location in Locations)
            {
                if (location.Revealed && location.Definition.Triggered != null)
                {
                    game = location.Definition.Triggered.ProcessEvent(game, nextEvent, location);
                }
            }

            return game;
        }

        public Game RecalculateOngoingEffects()
        {
            var ongoingCardAbilities = this.GetCardOngoingAbilities().ToList();
            var ongoingLocationAbilities = this.GetLocationOngoingAbilities().ToList();

            var recalculatedCards = this.AllCards.Select(c =>
                c with
                {
                    PowerAdjustment = this.GetPowerAdjustment(
                        c,
                        ongoingCardAbilities,
                        ongoingLocationAbilities,
                        this
                    )
                }
            );

            return this.WithUpdatedCards(recalculatedCards);
        }

        /// <summary>
        /// Calculates the total power adjustment to the given <see cref="Card"/>
        /// based on the pased-in list of all active ongoing abilities
        /// </summary>
        private int? GetPowerAdjustment(
            Card card,
            IReadOnlyList<(IOngoingAbility<Card> Ability, Card Source)> ongoingCardAbilities,
            IReadOnlyList<(
                IOngoingAbility<Location> Ability,
                Location Source
            )> ongoingLocationAbilities,
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

            foreach (var ongoing in ongoingLocationAbilities)
            {
                if (ongoing.Ability is OngoingAdjustPower<Location> adjustPower)
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
