using System.Collections.Immutable;
using Snapdragon.Events;
using Snapdragon.Fluent;
using Snapdragon.Fluent.Ongoing;
using Snapdragon.GameAccessors;
using Snapdragon.GameKernelAccessors;

namespace Snapdragon
{
    public record Game(
        Guid Id,
        GameKernel Kernel,
        Player TopPlayer,
        Player BottomPlayer,
        Side FirstRevealed,
        ImmutableList<Event> PastEvents,
        ImmutableList<Event> NewEvents,
        IGameLogger Logger,
        bool GameOver = false
    )
    {
        #region Accessors

        public int Turn => Kernel.Turn;

        public IPlayerAccessor Top => new TopPlayerAccessor(this, TopPlayer);

        public IPlayerAccessor Bottom => new BottomPlayerAccessor(this, BottomPlayer);

        /// <summary>
        /// Gets the <see cref="Location"/> for the given <see cref="Column"/>.
        public Location this[Column column] => Kernel[column];

        /// <summary>
        /// Gets the <see cref="Player"/> on the given <see cref="Side"/>.
        /// </summary>
        public IPlayerAccessor this[Side side]
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

        public Location Left => Kernel[Column.Left];

        public Location Middle => Kernel[Column.Middle];

        public Location Right => Kernel[Column.Right];

        public IEnumerable<Location> Locations
        {
            get
            {
                yield return Kernel[Column.Left];
                yield return Kernel[Column.Middle];
                yield return Kernel[Column.Right];
            }
        }

        /// <summary>
        /// Gets all <see cref="ICard"/>s that have been played and revealed.
        /// </summary>
        public IEnumerable<ICard> AllCards =>
            this.Locations.SelectMany(l => l.AllCards).Where(c => c.State == CardState.InPlay);

        /// <summary>
        /// Gets all <see cref="CardInstance"/>s that have been played, whether or not they are revealed.
        /// </summary>
        public IEnumerable<ICard> AllCardsIncludingUnrevealed =>
            this.Locations.SelectMany(l => l.AllCards);

        public IEnumerable<Sensor<ICard>> AllSensors =>
            this.Locations.SelectMany(l => l.AllSensors);

        public IEnumerable<(Ongoing<ICard> Ability, ICard Source)> GetCardOngoingAbilities()
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
            Ongoing<Location> Ability,
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

        public ICardInstance? GetCard(long cardId)
        {
            return Kernel[cardId];
        }

        /// <summary>
        /// Gets all revealed/in-play items that block other effects.
        /// </summary>
        public (
            IReadOnlyList<ICard> CardsWithLocationEffectBlocks,
            IReadOnlyList<ICard> CardsWithCardEffectBlocks,
            IReadOnlyList<Location> LocationsWithLocationEffectBlocks
        ) GetEffectBlockers()
        {
            var cardsWithLocationEffectBlocks = new List<ICard>();
            var cardsWithCardEffectBlocks = new List<ICard>();
            var locationsWithLocationEffectBlocks = new List<Location>();

            foreach (var card in AllCards)
            {
                if (card.Ongoing != null)
                {
                    if (card.Ongoing is OngoingBlockLocationEffect<ICard>)
                    {
                        cardsWithLocationEffectBlocks.Add(card);
                    }
                    else if (card.Ongoing is OngoingBlockCardEffect<ICard>)
                    {
                        cardsWithCardEffectBlocks.Add(card);
                    }
                }
            }

            foreach (var location in Locations)
            {
                if (
                    location.Definition.Ongoing != null
                    && location.Definition.Ongoing is OngoingBlockLocationEffect<Location>
                )
                {
                    locationsWithLocationEffectBlocks.Add(location);
                }
            }

            return (
                cardsWithLocationEffectBlocks,
                cardsWithCardEffectBlocks,
                locationsWithLocationEffectBlocks
            );
        }

        public IReadOnlySet<EffectType> GetBlockedEffects(
            Column column,
            Side side,
            IReadOnlyList<ICard>? cardsWithLocationEffectBlocks = null,
            IReadOnlyList<Location>? locationsWithLocationEffectBlocks = null
        )
        {
            var set = new HashSet<EffectType>();
            var location = this[column];

            foreach (var source in cardsWithLocationEffectBlocks ?? AllCards)
            {
                if (source.Ongoing is OngoingBlockLocationEffect<ICard> blockLocationEffect)
                {
                    // TODO: see if we can reduce the redundancy here
                    if (
                        blockLocationEffect.Selector.Get(source, this).Any(l => l.Column == column)
                        && blockLocationEffect
                            .PlayerSelector.Get(source, this)
                            .Any(p => p.Side == side)
                        && (blockLocationEffect.Condition?.IsMet(source, this) ?? true)
                    )
                    {
                        foreach (var blockedEffect in blockLocationEffect.BlockedEffects)
                        {
                            set.Add(blockedEffect);
                        }
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
                    // TODO: see if we can reduce the redundancy here
                    if (
                        blockLocationEffect.Selector.Get(loc, this).Any(l => l.Column == column)
                        && blockLocationEffect
                            .PlayerSelector.Get(loc, this)
                            .Any(p => p.Side == side)
                        && (blockLocationEffect.Condition?.IsMet(loc, this) ?? true)
                    )
                    {
                        foreach (var blockedEffect in blockLocationEffect.BlockedEffects)
                        {
                            set.Add(blockedEffect);
                        }
                    }
                }
            }

            return set;
        }

        /// <summary>
        /// Gets all location-wide effect blocks, by column.
        /// </summary>
        /// <param name="game">Overall game state.</param>
        /// <param name="cardsWithLocationEffectBlocks">All cards with <see cref="OngoingBlockLocationEffect{T}"/> abilities.</param>
        public IReadOnlyDictionary<Column, IReadOnlySet<EffectType>> GetBlockedEffectsByColumn(
            IReadOnlyList<ICard> cardsWithLocationEffectBlocks,
            IReadOnlyList<Location> locationsWithLocationEffectBlocks,
            Side side
        )
        {
            return All.Columns.ToDictionary(
                col => col,
                col =>
                    GetBlockedEffects(
                        col,
                        side,
                        cardsWithLocationEffectBlocks,
                        locationsWithLocationEffectBlocks
                    )
            );
        }

        /// <summary>
        /// Gets the types of effects that are blocked for the given card,
        /// including those blocked based on its current location.
        ///
        /// Optional parameters are passed in for optimization (primarily for ControllerUtilities).
        /// </summary>
        public IReadOnlySet<EffectType> GetBlockedEffects(
            ICard card,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>>? blockedEffectsByColumn = null,
            IReadOnlyList<ICard>? cardsWithCardEffectBlocks = null
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
                    // TODO: see if we can reduce the redundancy here
                    if (blockCardEffect.Selector.Get(source, this).Any(c => c.Id == card.Id))
                    {
                        foreach (var blockedEffect in blockCardEffect.BlockedEffects)
                            set.Add(blockedEffect);
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
            ICard card,
            Column destination,
            IReadOnlyDictionary<Column, IReadOnlySet<EffectType>>? blockedEffectsByColumn = null,
            IReadOnlyList<ICard>? cardsWithMoveAbilities = null,
            IReadOnlyList<Sensor<ICard>>? sensorsWithMoveAbilities = null,
            IReadOnlyList<ICard>? cardsWithCardEffectBlocks = null
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
                    return this with { TopPlayer = player };
                case Side.Bottom:
                    return this with { BottomPlayer = player };
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
                var game = this with { Kernel = Kernel.DrawCard(side) };
                game = game.WithEvent(new CardDrawnEvent(Turn, game[player.Side].Hand.Last()));

                return game;
            }

            return this;
        }

        public Game DrawOpponentCard(Side side)
        {
            var player = this[side];

            // TODO: Check for any blocks on drawing cards
            if (player.Library.Count > 0 && player.Hand.Count < Max.HandSize)
            {
                var game = this with { Kernel = Kernel.DrawOpponentCard(side) };
                game = game.WithEvent(new CardDrawnEvent(Turn, game[player.Side].Hand.Last()));
                return game;
            }

            return this;
        }

        public Game WithRevealedLocation(Column column)
        {
            var game = this with { Kernel = Kernel.RevealLocation(column) };
            var location = game[column];
            if (location.Definition.OnReveal != null)
            {
                game = location.Definition.OnReveal.Apply(location, game).Apply(game);
            }
            return game.WithEvent(new LocationRevealedEvent(game.Turn, game[column]))
                .RecalculateMultipliers();
        }

        public Game WithNewCardInPlay(CardDefinition cardDefinition, Column column, Side side)
        {
            // TODO: Raise an event for this
            var game = this with
            {
                Kernel = Kernel.AddNewCardToLocation(cardDefinition, column, side, out long _)
            };
            return game.RecalculateMultipliers();
        }

        public Game WithNewCardInHand(CardDefinition cardDefinition, Side side)
        {
            var game = this with
            {
                Kernel = Kernel.AddNewCardToHand(cardDefinition, side, out long newCardId)
            };

            return game.WithEvent(new CardAddedToHandEvent(game.GetCard(newCardId), game.Turn));
        }

        public Game WithCopyInPlay(ICardInstance card, Column column, Side side)
        {
            var game = this with
            {
                Kernel = Kernel.AddCopiedCardToLocation(card.Id, column, side, out long _)
            };

            return game.RecalculateMultipliers(); // TODO: Add event for card being placed here
        }

        public Game WithCopyInHand(ICardInstance card, Side side, ICardTransform? transform)
        {
            var game = this with
            {
                Kernel = Kernel.AddCopiedCardToHand(card.Id, side, out long newCardId)
            };

            var newCard = game.GetCard(newCardId);
            if (transform != null)
            {
                var transformedCard = transform.Apply(newCard.Base, card); // TODO: Determine if this is ever the wrong "source" for a transform
                game = game.WithUpdatedCard(transformedCard);
            }

            return game.WithEvent(new CardAddedToHandEvent(game.GetCard(newCardId), game.Turn));
        }

        public Game WithCardDiscarded(ICardInstance card)
        {
            card = Kernel[card.Id];
            if (card == null)
            {
                throw new InvalidOperationException($"Card {card.Name} ({card.Id}) not found.");
            }

            var game = this with { Kernel = Kernel.DiscardCard(card.Id, card.Side) };
            return game.WithEvent(new CardDiscardedEvent(game.Turn, card));
        }

        public Game DestroyCardInPlay(ICardInstance card)
        {
            var actualCard = Kernel[card.Id] as ICard;
            if (actualCard == null)
            {
                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) not found in any location."
                );
            }

            var game = this with
            {
                Kernel = Kernel.DestroyCardFromPlay(
                    actualCard.Id,
                    actualCard.Column,
                    actualCard.Side
                )
            };

            return game.WithEvent(new CardDestroyedFromPlayEvent(game.Turn, actualCard))
                .RecalculateMultipliers();
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{ICardInLocation}"/> added.
        /// </summary>
        public Game WithSensor(Sensor<ICard> sensor)
        {
            return this with { Kernel = Kernel.AddSensor(sensor) };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{ICardInLocation}"/> removed.
        /// </summary>
        public Game WithSensorDeleted(long sensorId)
        {
            var sensor =
                Kernel.Sensors.GetValueOrDefault(sensorId)
                ?? throw new InvalidOperationException($"Sensor {sensorId} not found.");

            return this with
            {
                Kernel = Kernel.DestroySensor(sensor.Id, sensor.Column, sensor.Side)
            };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="CardBase"/>s updated (in place).
        /// Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        public Game WithUpdatedCards(IEnumerable<CardBase> cards)
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
        /// Gets a modified state with the given <see cref="ICardInstance"/> updated (in place).
        /// Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        public Game WithUpdatedCard(ICardInstance card)
        {
            var game = this with { Kernel = Kernel.WithUpdatedCard(card) };
            return game.RecalculateMultipliers();
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="CardBase"/> updated (in place).
        /// Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        public Game WithUpdatedCard(CardBase card)
        {
            var game = this with { Kernel = Kernel.WithUpdatedCard(card) };
            return game.RecalculateMultipliers();
        }

        public Game SwitchCardSide(ICard card)
        {
            var game = this with { Kernel = Kernel.SwitchCardSide(card.Id, card.Side) };
            var modifiedCard = game.GetCard(card.Id);
            return game.WithEvent(new CardSwitchedSidesEvent(modifiedCard, game.Turn))
                .RecalculateMultipliers();
        }

        public Game ReturnCardToHand(ICardInstance card)
        {
            // TODO: Raise an event for this
            var game = this with
            {
                Kernel = Kernel.ReturnCardToHand(card.Id, card.Side)
            };
            return game.RecalculateMultipliers();
        }

        public Game ReturnDiscardToPlay(ICardInstance card, Column column)
        {
            // TODO: Raise an event for this
            var game = this with
            {
                Kernel = Kernel.ReturnDiscardToLocation(card.Id, column, card.Side)
            };
            return game.RecalculateMultipliers();
        }

        public Game ReturnDestroyedToPlay(ICardInstance card, Column column)
        {
            // TODO: Raise an event for this
            var game = this with
            {
                Kernel = Kernel.ReturnDestroyedToLocation(card.Id, column, card.Side)
            };
            return game.RecalculateMultipliers();
        }

        /// <summary>
        /// Removes the card from the game entirely. At the moment this is ONLY intended for card merges.
        /// </summary>
        /// <param name="card">Card to remove.</param>
        public Game RemoveCard(ICardInstance card)
        {
            var game = this with { Kernel = Kernel.RemoveCardFromGame(card.Id) };
            return game.RecalculateMultipliers();
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
            game = game.RecalculatePower();

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
        /// Triggers the on-reveal ability of a card (multiple times, if appropriate).
        ///
        /// This is automatically done within <see cref="RevealCard(ICard)"> but is exposed
        /// so it can also be done by Odin's on-reveal ability.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game TriggerOnRevealAbility(ICard card)
        {
            var game = this;

            // Note this is guaranteed to be valid based on the checks in Kernel.RevealCard(...).
            card = (ICard)Kernel[card.Id];

            var multiplier = game[card.Column].Multipliers[card.Side].OnReveal;

            if (card.OnReveal != null)
            {
                for (var i = 0; i < multiplier; i++)
                {
                    game = card.OnReveal.Apply(card, game).Apply(game);
                }
            }

            return game;
        }

        /// <summary>
        /// Helper function that reveals a single card, then processes any triggered events.
        /// </summary>
        private Game RevealCard(ICard card)
        {
            var game = this with { Kernel = Kernel.RevealCard(card.Id) };

            game = game.TriggerOnRevealAbility(card);

            // This is to ensure that cards that get modified by their own reveal
            // abilities get attached to the reveal event in their modified state,
            // which may or may not be super useful at this point.
            //
            // The null coalesce operator is because of Hulkbuster.
            var revealedCard = (ICard)Kernel[card.Id] ?? card;

            return game.WithEvent(new CardRevealedEvent(game.Turn, revealedCard))
                .RecalculateMultipliers()
                .ProcessEvents();
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
                Kernel = Kernel with { Turn = Kernel.Turn + 1 }
            };
            game = game.RevealLocation();
            game = game.ProcessEvents();

            // Each Player draws a card, and gets an amount of energy equal to the turn count
            game = game.DrawCard(Side.Top).DrawCard(Side.Bottom);
            var topPlayer = game.TopPlayer with { Energy = game.Turn };
            var bottomPlayer = game.BottomPlayer with { Energy = game.Turn };

            game = game with { TopPlayer = topPlayer, BottomPlayer = bottomPlayer };

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
            foreach (var cardWithTrigger in originalState.Left.TopCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var cardWithTrigger in originalState.Left.BottomCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var cardWithTrigger in originalState.Middle.TopCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var cardWithTrigger in originalState.Middle.BottomCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var cardWithTrigger in originalState.Right.TopCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var cardWithTrigger in originalState.Right.BottomCards)
            {
                if (cardWithTrigger.State == CardState.InPlay && cardWithTrigger.Triggered != null)
                {
                    game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
                }
            }

            foreach (var discardedOrDestroyedCard in originalState.Top.Discards)
            {
                if (discardedOrDestroyedCard.Triggered?.DiscardedOrDestroyed() ?? false)
                {
                    game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                        game,
                        nextEvent,
                        discardedOrDestroyedCard
                    );
                }
            }

            foreach (var discardedOrDestroyedCard in originalState.Top.Destroyed)
            {
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed())
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            foreach (var discardedOrDestroyedCard in originalState.Bottom.Discards)
            {
                if (discardedOrDestroyedCard.Triggered?.DiscardedOrDestroyed() ?? false)
                {
                    game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                        game,
                        nextEvent,
                        discardedOrDestroyedCard
                    );
                }
            }

            foreach (var discardedOrDestroyedCard in originalState.Bottom.Destroyed)
            {
                if (discardedOrDestroyedCard.Triggered != null)
                {
                    if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed())
                    {
                        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
                            game,
                            nextEvent,
                            discardedOrDestroyedCard
                        );
                    }
                }
            }

            foreach (var cardInHand in originalState.Top.Hand)
            {
                if (cardInHand.Triggered != null)
                {
                    if (cardInHand.Triggered.InHand())
                    {
                        game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
                    }
                }
            }

            foreach (var cardInHand in originalState.Bottom.Hand)
            {
                if (cardInHand.Triggered != null)
                {
                    if (cardInHand.Triggered.InHand())
                    {
                        game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
                    }
                }
            }

            foreach (var cardInLibrary in originalState.Top.Library)
            {
                if (cardInLibrary.Triggered != null)
                {
                    if (cardInLibrary.Triggered.InDeck())
                    {
                        game =
                            cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary)
                            ?? game;
                    }
                }
            }

            foreach (var cardInLibrary in originalState.Bottom.Library)
            {
                if (cardInLibrary.Triggered != null)
                {
                    if (cardInLibrary.Triggered.InDeck())
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

        public Game PlayCard(ICardInstance card, Column column)
        {
            var game = this with { Kernel = Kernel.PlayCard(card.Id, column, card.Side) };
            card = game.Kernel[card.Id];

            return game.WithEvent(new CardPlayedEvent(this.Turn, card));
        }

        public Game MoveCard(ICard card, Column to)
        {
            var game = this with { Kernel = Kernel.MoveCard(card.Id, card.Side, card.Column, to) };
            game = game.WithEvent(new CardMovedEvent(game.Turn, Kernel[card.Id], card.Column, to));
            game = game.RecalculateMultipliers();

            return game;
        }

        /// <summary>
        /// Recalculates any multipliers for effects (e.g., Wong, Onslaught).
        /// </summary>
        public Game RecalculateMultipliers()
        {
            var effectBlockers = GetEffectBlockers();

            var kernel = this.Kernel;

            foreach (var location in Locations)
            {
                var onRevealBase = 1;
                var ongoingBase = 1;

                if (location.Revealed)
                {
                    if (location.Definition.Ongoing is OngoingDoubleOnReveal<Location>)
                    {
                        onRevealBase = 2;
                    }
                    else if (location.Definition.Ongoing is OngoingDoubleOtherOngoing<Location>)
                    {
                        ongoingBase = 2;
                    }
                }

                var onRevealTop = onRevealBase;
                var onRevealBottom = onRevealBase;
                var ongoingTop = ongoingBase;
                var ongoingBottom = ongoingBase;

                var blockedEffectsTop = GetBlockedEffects(
                    location.Column,
                    Side.Top,
                    effectBlockers.CardsWithLocationEffectBlocks,
                    effectBlockers.LocationsWithLocationEffectBlocks
                );
                var blockedEffectsBottom = GetBlockedEffects(
                    location.Column,
                    Side.Bottom,
                    effectBlockers.CardsWithLocationEffectBlocks,
                    effectBlockers.LocationsWithLocationEffectBlocks
                );

                if (blockedEffectsTop.Contains(EffectType.OnRevealAbilities))
                {
                    onRevealTop = 0;
                }
                if (blockedEffectsTop.Contains(EffectType.OngoingAbilities))
                {
                    ongoingTop = 0;
                }
                if (blockedEffectsBottom.Contains(EffectType.OnRevealAbilities))
                {
                    onRevealBottom = 0;
                }
                if (blockedEffectsBottom.Contains(EffectType.OngoingAbilities))
                {
                    ongoingBottom = 0;
                }

                // We have to figure out ongoing multipliers first, because Wong's ability
                // is itself an ongoing ability, and can therefore be multiplied
                foreach (var card in location[Side.Top])
                {
                    if (card.State == CardState.InPlay)
                    {
                        if (card.Ongoing is OngoingDoubleOtherOngoing<ICard>)
                        {
                            ongoingTop *= 2;
                        }
                    }
                }

                foreach (var card in location[Side.Bottom])
                {
                    if (card.State == CardState.InPlay)
                    {
                        if (card.Ongoing is OngoingDoubleOtherOngoing<ICard>)
                        {
                            ongoingBottom *= 2;
                        }
                    }
                }

                foreach (var card in location[Side.Top])
                {
                    if (card.State == CardState.InPlay)
                    {
                        if (card.Ongoing is OngoingDoubleOnReveal<ICard>)
                        {
                            for (var i = 0; i < ongoingTop; i++)
                            {
                                onRevealTop *= 2;
                            }
                        }
                    }
                }

                foreach (var card in location[Side.Bottom])
                {
                    if (card.State == CardState.InPlay)
                    {
                        if (card.Ongoing is OngoingDoubleOnReveal<ICard>)
                        {
                            for (var i = 0; i < ongoingBottom; i++)
                            {
                                onRevealBottom *= 2;
                            }
                        }
                    }
                }

                var topMultipliers = new Multipliers(onRevealTop, ongoingTop);
                var bottomMultipliers = new Multipliers(onRevealBottom, ongoingBottom);

                if (topMultipliers != location.TopMultipliers)
                {
                    kernel = kernel.UpdateMultipliers(location.Column, Side.Top, topMultipliers);
                }

                if (bottomMultipliers != location.BottomMultipliers)
                {
                    kernel = kernel.UpdateMultipliers(
                        location.Column,
                        Side.Bottom,
                        bottomMultipliers
                    );
                }
            }

            return this with
            {
                Kernel = kernel
            };
        }

        public Game RecalculatePower()
        {
            var recalculatedCards = new List<CardBase>();

            var blockers = GetEffectBlockers();
            var topBlockedEffectsByColumn = GetBlockedEffectsByColumn(
                blockers.CardsWithLocationEffectBlocks,
                blockers.LocationsWithLocationEffectBlocks,
                Side.Top
            );
            var bottomBlockedEffectsByColumn = GetBlockedEffectsByColumn(
                blockers.CardsWithLocationEffectBlocks,
                blockers.LocationsWithLocationEffectBlocks,
                Side.Top
            );

            var ongoingCardAbilities = this.GetCardOngoingAbilities().ToList();
            var ongoingLocationAbilities = this.GetLocationOngoingAbilities().ToList();

            foreach (var card in this.AllCards)
            {
                var blockedEffects = GetBlockedEffects(
                    card,
                    card.Side == Side.Top
                        ? topBlockedEffectsByColumn
                        : bottomBlockedEffectsByColumn,
                    blockers.CardsWithCardEffectBlocks
                );

                // "Power" is the power from the Definition plus any applicable modifications
                var power = card.Definition.Power;

                foreach (var mod in card.Base.Modifications)
                {
                    if (mod.PowerChange != null)
                    {
                        if (mod.PowerChange < 0 && blockedEffects.Contains(EffectType.ReducePower))
                        {
                            continue;
                        }

                        power += mod.PowerChange.Value;
                    }
                }

                var powerAdjustment = this.GetPowerAdjustment(
                    card,
                    ongoingCardAbilities,
                    ongoingLocationAbilities,
                    blockedEffects.Contains(EffectType.ReducePower)
                );

                if (power != card.Base.Power || powerAdjustment != card.Base.PowerAdjustment)
                {
                    recalculatedCards.Add(
                        card.Base with
                        {
                            Power = power,
                            PowerAdjustment = powerAdjustment
                        }
                    );
                }
            }

            if (recalculatedCards.Count > 0)
            {
                return this.WithUpdatedCards(recalculatedCards);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Calculates the total power adjustment to the given <see cref="Card"/>
        /// based on the pased-in list of all active ongoing abilities
        /// </summary>
        private int? GetPowerAdjustment(
            ICard card,
            IReadOnlyList<(Ongoing<ICard> Ability, ICard Source)> ongoingCardAbilities,
            IReadOnlyList<(Ongoing<Location> Ability, Location Source)> ongoingLocationAbilities,
            bool cannotReduce = false
        )
        {
            var any = false;
            var total = 0;

            foreach (var ongoing in ongoingCardAbilities)
            {
                if (ongoing.Ability is OngoingAdjustPower<ICard> adjustPower)
                {
                    var ongoingMultiplier = this[ongoing.Source.Column]
                        .Multipliers[ongoing.Source.Side]
                        .Ongoing;

                    if (adjustPower.Amount < 0 && cannotReduce)
                    {
                        continue;
                    }

                    if (ongoingMultiplier == 0) // Sorta redundant, but would presumably save time
                    {
                        continue;
                    }

                    // TODO: Reduce redundancy here
                    if (adjustPower.Selector.Get(ongoing.Source, this).Any(c => c.Id == card.Id))
                    {
                        total += adjustPower.Amount * ongoingMultiplier;
                        any = true;
                    }
                }
            }

            foreach (var ongoing in ongoingLocationAbilities)
            {
                if (ongoing.Ability is OngoingAdjustPower<Location> adjustPower)
                {
                    if (adjustPower.Amount < 0 && cannotReduce)
                    {
                        continue;
                    }

                    // TODO: Reduce redundancy here
                    if (adjustPower.Selector.Get(ongoing.Source, this).Any(c => c.Id == card.Id))
                    {
                        total += adjustPower.Amount;
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
                    if (card.Ongoing is OngoingAdjustLocationPower<ICard> addLocationPower)
                    {
                        var ongoingMultiplier = this[card.Column].Multipliers[card.Side].Ongoing;

                        if (ongoingMultiplier == 0) // Somewhat redundant, but should be faster
                        {
                            continue;
                        }

                        if (
                            addLocationPower
                                .Selector.Get(card, this)
                                .Any(l => l.Column == location.Column)
                        )
                        {
                            // TODO: Deal with the fact that the card isn't the "target"
                            var power = addLocationPower.Amount * ongoingMultiplier;

                            // TODO: Check if anything adds power to the opposite side (probably the case)
                            scores = scores.WithAddedPower(power, column, card.Side);
                        }
                    }
                }

                // Now handle the special "double power" ability
                foreach (var side in All.Sides)
                {
                    var powerMultiplier = 1;
                    var ongoingMultiplier = this[column].Multipliers[side].Ongoing;

                    if (ongoingMultiplier == 0)
                    {
                        continue;
                    }

                    foreach (var card in location[side])
                    {
                        if (card.Ongoing is OngoingDoubleLocationPower)
                        {
                            for (var i = 0; i < ongoingMultiplier; i++)
                            {
                                powerMultiplier *= 2;
                            }
                        }
                    }

                    if (powerMultiplier > 1)
                    {
                        scores = scores.WithAddedPower(
                            scores[column][side] * (powerMultiplier - 1),
                            column,
                            side
                        );
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
