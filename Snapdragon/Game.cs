using System.Collections.Immutable;
using Snapdragon.Events;
using Snapdragon.Fluent;
using Snapdragon.Fluent.Ongoing;
using Snapdragon.PlayerActions;

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
        ImmutableDictionary<long, ICardInstance> CardsById,
        ImmutableDictionary<long, EventType> CardEventTriggers,
        ImmutableDictionary<EventType, ImmutableHashSet<long>> CardsByTriggerEventType,
        ImmutableDictionary<long, Sensor<ICard>> SensorsById,
        ImmutableDictionary<long, EventType> SensorEventTriggers,
        ImmutableDictionary<EventType, ImmutableHashSet<long>> SensorsByTriggerEventType,
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
        public IEnumerable<ICard> AllCards =>
            this
                .Left.AllCardsIncludingUnrevealed.Concat(this.Middle.AllCardsIncludingUnrevealed)
                .Concat(this.Right.AllCardsIncludingUnrevealed)
                .Where(c => c.State == CardState.InPlay);

        public ICardInstance? GetCardInstance(long cardId)
        {
            return CardsById.GetValueOrDefault(cardId);
        }

        public ICard? GetCard(long cardId)
        {
            var cardInstance = CardsById.GetValueOrDefault(cardId);

            if (cardInstance is Card card)
            {
                return card;
            }

            return null;
        }

        public Sensor<ICard>? GetSensor(long sensorId)
        {
            return AllSensors.SingleOrDefault(s => s.Id == sensorId);
        }

        /// <summary>
        /// Gets all <see cref="CardInstance"/>s that have been played, whether or not they are revealed.
        /// </summary>
        public IEnumerable<ICard> AllCardsIncludingUnrevealed =>
            this
                .Left.AllCardsIncludingUnrevealed.Concat(this.Middle.AllCardsIncludingUnrevealed)
                .Concat(this.Right.AllCardsIncludingUnrevealed);

        public IEnumerable<Sensor<ICard>> AllSensors =>
            this.Left.Sensors.Concat(this.Middle.Sensors).Concat(this.Right.Sensors);

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

        /// <summary>
        /// Gets all revealed/in-play items that block other effects.
        /// </summary>
        public (
            IReadOnlyList<ICard> CardsWithLocationEffectBlocks,
            IReadOnlyList<ICard> CardsWithCardEffectBlocks,
            IReadOnlyList<Location> LocationsWithLocationEffectBlocks
        ) GetEffectBlockers(IReadOnlyList<ICard> cards = null)
        {
            cards ??= this.AllCards.ToList();

            var cardsWithLocationEffectBlocks = new List<ICard>();
            var cardsWithCardEffectBlocks = new List<ICard>();
            var locationsWithLocationEffectBlocks = new List<Location>();

            foreach (var card in cards)
            {
                if (card.Ongoing != null)
                {
                    if (card.Ongoing.Type == OngoingAbilityType.BlockLocationEffects)
                    {
                        cardsWithLocationEffectBlocks.Add(card);
                    }
                    else if (card.Ongoing.Type == OngoingAbilityType.BlockCardEffects)
                    {
                        cardsWithCardEffectBlocks.Add(card);
                    }
                }
            }

            foreach (var location in Locations)
            {
                if (
                    location.Definition.Ongoing != null
                    && location.Definition.Ongoing.Type == OngoingAbilityType.BlockLocationEffects
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
            var player = this[side];

            foreach (var source in cardsWithLocationEffectBlocks ?? AllCards)
            {
                if (source.Ongoing is OngoingBlockLocationEffect<ICard> blockLocationEffect)
                {
                    if (
                        blockLocationEffect.Selector.Selects(location, source, this)
                        && blockLocationEffect.PlayerSelector.Selects(player, source, this)
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
                    if (
                        blockLocationEffect.Selector.Selects(location, loc, this)
                        && blockLocationEffect.PlayerSelector.Selects(player, loc, this)
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
                if (source.Ongoing?.Type == OngoingAbilityType.BlockCardEffects)
                {
                    var blockCardEffect = source.Ongoing as OngoingBlockCardEffect<ICard>;

                    if (blockCardEffect.Selector.Selects(card, source, this))
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

        public Game DrawCard(Side side)
        {
            var player = this[side];

            // TODO: Check for any blocks on drawing cards
            if (player.Library.Count > 0 && player.Hand.Count < Max.HandSize)
            {
                player = player.DrawCard();
                var drawnCard = player.Hand.Last();
                return this.WithPlayer(player)
                    .WithUpdatedCards(drawnCard)
                    .WithEvent(new CardDrawnEvent(Turn, drawnCard));
            }

            return this;
        }

        public Game DrawOpponentCard(Side side)
        {
            var player = this[side];
            var opponent = this[side.Other()];

            // TODO: Check for any blocks on drawing cards
            if (opponent.Library.Count > 0 && player.Hand.Count < Max.HandSize)
            {
                var card = opponent.Library[0].ToCardInstance() with
                {
                    Side = side,
                    State = CardState.InHand
                };
                opponent = opponent with { Library = opponent.Library.RemoveAt(0) };
                player = player with { Hand = player.Hand.Add(card) };

                return this.WithPlayer(player)
                    .WithPlayer(opponent)
                    .WithUpdatedCards(card)
                    .WithEvent(new CardDrawnEvent(Turn, card));
            }

            return this;
        }

        public Game WithRevealedLocation(Column column)
        {
            var location = this[column] with { Revealed = true };

            var game = this.WithLocation(location)
                .WithEvent(new LocationRevealedEvent(this.Turn, location));

            if (location.Definition.OnReveal != null)
            {
                game = location.Definition.OnReveal.Apply(location, game).Apply(game);
            }
            game = game.WithEvent(new LocationRevealedEvent(game.Turn, game[column]));

            if (
                location.Definition.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || location.Definition.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || location.Definition.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || location.Definition.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        /// <summary>
        /// Adds a new card to a particular location and side.
        ///
        /// Does not validate that this is appropriate; caller must orchestrate whatever is happening correctly.
        /// </summary>
        /// <param name="cardDefinition"></param>
        /// <param name="column"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public Game WithNewCardInPlayUnsafe(CardDefinition cardDefinition, Column column, Side side)
        {
            var newCard = new Card(cardDefinition, side, column);

            var location = this[column];
            var game = this.WithLocation(location.WithCard(newCard)).WithUpdatedCards(newCard); // TODO: Raise an event for this

            // For performance reasons, skip recalculation if the card doesn't affect anything
            if (
                cardDefinition.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || cardDefinition.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || cardDefinition.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || cardDefinition.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        public Game WithNewCardInHandUnsafe(CardDefinition cardDefinition, Side side)
        {
            var newCard = new CardInstance(cardDefinition, side, CardState.InHand);
            var player = this[side];

            var hand = player.Hand.Add(newCard);
            player = player with { Hand = hand };
            var game = this.WithPlayer(player);

            return game.WithEvent(new CardAddedToHandEvent(newCard, game.Turn))
                .WithUpdatedCards(newCard);
        }

        public Game WithNewCardInDeckUnsafe(CardDefinition cardDefinition, Side side)
        {
            var newCard = new CardInstance(cardDefinition, side, CardState.InLibrary);
            var player = this[side];

            var library = player.Library with { Cards = player.Library.Cards.Add(newCard) };
            player = player with { Library = library };
            var game = this.WithPlayer(player).WithUpdatedCards(newCard);

            return game; // TODO: Determine what event should be raised, if any
        }

        public Game WithCopyInPlayUnsafe(ICardInstance card, Column column, Side side)
        {
            var copy = card.ToCardInstance() with { Id = Ids.GetNextCard(), Side = side };
            var location = this[column].WithCard(copy.InPlayAt(column));

            var game = this.WithLocation(location).WithUpdatedCards(copy);

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers(); // TODO: Add event for card being placed here
            }
            else
            {
                return game; // TODO: Add event for card being placed here
            }
        }

        public Game WithCopyInHandUnsafe(ICardInstance card, Side side, ICardTransform? transform)
        {
            var copy = card.ToCardInstance() with { Id = Ids.GetNextCard(), Side = side };

            if (transform != null)
            {
                copy = transform.Apply(copy).ToCardInstance();
            }

            var player = this[side];

            var hand = player.Hand.Add(copy);
            player = player with { Hand = hand };
            var game = this.WithPlayer(player).WithUpdatedCards(copy);

            return game.WithEvent(new CardAddedToHandEvent(copy, game.Turn));
        }

        public Game WithCardDiscarded(ICardInstance card)
        {
            var player = this[card.Side];
            var cardInHand = player.Hand.Single(c => c.Id == card.Id);
            var cardDiscarded = cardInHand.ToCardInstance() with { State = CardState.Discarded };
            player = player with
            {
                Hand = player.Hand.Remove(cardInHand),
                Discards = player.Discards.Add(cardDiscarded)
            };

            return this.WithPlayer(player)
                .WithUpdatedCards(cardDiscarded)
                .WithEvent(new CardDiscardedEvent(this.Turn, cardDiscarded));
        }

        public Game DestroyCardInPlay(ICardInstance card)
        {
            var actualCard = this.AllCards.Single(c => c.Id == card.Id);

            if (actualCard == null)
            {
                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) not found in any location."
                );
            }

            var location = this[actualCard.Column].WithRemovedCard(actualCard);
            var destroyedCard = actualCard.ToCardInstance() with { State = CardState.Destroyed };
            var player = this[actualCard.Side];
            player = player with { Destroyed = player.Destroyed.Add(destroyedCard) };
            var game = this.WithLocation(location)
                .WithPlayer(player)
                .WithUpdatedCards(destroyedCard)
                .WithEvent(new CardDestroyedFromPlayEvent(this.Turn, actualCard));

            if (
                actualCard.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || actualCard.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || actualCard.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || actualCard.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{ICard}"/>.  Note that unlike <see
        /// cref="WithCard(CardInstance)"/>, this adds a new Sensor rather than modifying an existing one.
        /// </summary>
        public Game WithSensor(Sensor<ICard> sensor)
        {
            var location = this[sensor.Column];

            var sensorsByTriggerEventType = SensorsByTriggerEventType;

            if (sensor.TriggeredAbility?.EventType != null)
            {
                var sensorsWithTrigger =
                    SensorsByTriggerEventType.GetValueOrDefault(sensor.TriggeredAbility.EventType)
                    ?? ImmutableHashSet<long>.Empty;
                sensorsWithTrigger = sensorsWithTrigger.Add(sensor.Id);
                sensorsByTriggerEventType = SensorsByTriggerEventType.SetItem(
                    sensor.TriggeredAbility.EventType,
                    sensorsWithTrigger
                );
            }

            return this.WithLocation(location.WithSensor(sensor)) with
            {
                SensorsById = this.SensorsById.SetItem(sensor.Id, sensor),
                SensorsByTriggerEventType = sensorsByTriggerEventType
            };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="Sensor{ICard}"/> removed.
        /// </summary>
        public Game WithSensorDeleted(long sensorId)
        {
            var sensor = this.GetSensor(sensorId);

            var sensorsByTriggerEventType = SensorsByTriggerEventType;

            if (sensor.TriggeredAbility?.EventType != null)
            {
                var sensorsWithTrigger = SensorsByTriggerEventType[
                    sensor.TriggeredAbility.EventType
                ];
                sensorsWithTrigger = sensorsWithTrigger.Remove(sensorId);
                sensorsByTriggerEventType = sensorsByTriggerEventType.SetItem(
                    sensor.TriggeredAbility.EventType,
                    sensorsWithTrigger
                );
            }

            var sensorsById = SensorsById.Remove(sensorId);

            return this with
            {
                Left = this.Left.WithSensorDeleted(sensorId),
                Middle = this.Middle.WithSensorDeleted(sensorId),
                Right = this.Right.WithSensorDeleted(sensorId),
                SensorsById = sensorsById,
                SensorsByTriggerEventType = sensorsByTriggerEventType
            };
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="ICard"/>s updated.  Currently only suitable for cards in
        /// play, with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithModifiedCards(IEnumerable<ICard> cards)
        {
            // TODO: Determine if this needs to be optimized
            var game = this;

            foreach (var card in cards)
            {
                game = game.WithModifiedCard(card);
            }

            return game;
        }

        /// <summary>
        /// Gets a modified state with the given <see cref="ICard"/> updated.  Currently only suitable for cards in play,
        /// with attributes (typically PowerAdjustment) changed. Cannot handle moved cards, destroyed cards, etc.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Game WithModifiedCard(ICardInstance card)
        {
            var game = this.WithUpdatedCards(card);

            if (card.Column != null)
            {
                var location = this[card.Column.Value];
                var newCards = location[card.Side]
                    .Select(c => c.Id == card.Id ? card.InPlayAt(card.Column.Value) : c)
                    .ToImmutableList();

                location = location with
                {
                    TopCardsIncludingUnrevealed =
                        card.Side == Side.Top ? newCards : location.TopCardsIncludingUnrevealed,
                    BottomCardsIncludingUnrevealed =
                        card.Side == Side.Bottom
                            ? newCards
                            : location.BottomCardsIncludingUnrevealed,
                };

                game = game.WithLocation(location);
            }
            else if (card.State == CardState.InLibrary)
            {
                var player = game[card.Side];
                player = player with
                {
                    Library = player.Library with
                    {
                        Cards = player
                            .Library.Cards.Select(c => c.Id == card.Id ? card : c)
                            .ToImmutableList()
                    }
                };
                game = game.WithPlayer(player);
            }
            else if (card.State == CardState.InHand)
            {
                var player = game[card.Side];
                player = player with
                {
                    Hand = player.Hand.Select(c => c.Id == card.Id ? card : c).ToImmutableList()
                };
                game = game.WithPlayer(player);
            }

            return game;
        }

        public Game SwitchCardSideUnsafe(ICard card)
        {
            var actualCard = this.AllCards.Single(c => c.Id == card.Id);
            var location = this[actualCard.Column];
            location = location.WithoutCard(actualCard);
            var switchedCard = (
                actualCard.ToCardInstance() with
                {
                    Side = actualCard.Side.Other()
                }
            ).InPlayAt(actualCard.Column);
            location = location.WithCard(switchedCard);

            var game = this.WithLocation(location);

            game = game.WithEvent(new CardSwitchedSidesEvent(switchedCard, game.Turn))
                .WithUpdatedCards(switchedCard);

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        public Game ReturnCardToHandUnsafe(ICardInstance card)
        {
            var game = this;
            var actualCard = this.GetCardInstance(card.Id);

            if (actualCard?.Column != null)
            {
                var location = this[actualCard.Column.Value];
                game = game.WithLocation(location.WithoutCard(actualCard));
            }

            var player = this[actualCard.Side];
            var cardInHand = actualCard.ToCardInstance() with { State = CardState.InHand };
            player = player with { Hand = player.Hand.Add(cardInHand) };

            switch (actualCard.State)
            {
                case CardState.Discarded:
                    player = player with
                    {
                        Discards = player.Discards.RemoveAll(c => c.Id == card.Id)
                    };
                    break;
                case CardState.Destroyed:
                    player = player with
                    {
                        Destroyed = player.Destroyed.RemoveAll(c => c.Id == card.Id)
                    };
                    break;
            }

            // TODO: Raise an event for this
            game = game.WithPlayer(player).WithUpdatedCards(cardInHand);

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        public Game ReturnDiscardToPlayUnsafe(ICardInstance card, Column column)
        {
            var player = this[card.Side];
            var actualCard = player.Discards.Single(c => c.Id == card.Id);
            player = player with { Discards = player.Discards.Remove(actualCard) };
            var returnedCard = actualCard.InPlayAt(column) with { State = CardState.InPlay };
            var location = this[column].WithCard(returnedCard);

            // TODO: Raise an event for this
            var game = this.WithLocation(location)
                .WithPlayer(player)
                .WithUpdatedCards(returnedCard);

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        public Game ReturnDestroyedToPlay(ICardInstance card, Column column)
        {
            var player = this[card.Side];
            var actualCard = player.Destroyed.Single(c => c.Id == card.Id);
            player = player with { Destroyed = player.Destroyed.Remove(actualCard) };
            var returnedCard = actualCard.InPlayAt(column) with { State = CardState.InPlay };
            var location = this[column].WithCard(returnedCard);

            // TODO: Raise an event for this
            var game = this.WithLocation(location).WithUpdatedCards(returnedCard);

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        /// <summary>
        /// Removes the card from the game entirely. At the moment this is ONLY intended for card merges.
        /// </summary>
        /// <param name="card">Card to remove.</param>
        public Game RemoveCard(ICardInstance card)
        {
            var actualCard = this.AllCards.Single(c => c.Id == card.Id);
            var location = this[actualCard.Column];

            // TODO: Raise an event for this
            var game = this.WithLocation(location.WithRemovedCard(actualCard));

            if (actualCard.Triggered?.EventType != null)
            {
                game = game with
                {
                    CardsByTriggerEventType = CardsByTriggerEventType.SetItem(
                        actualCard.Triggered.EventType,
                        CardsByTriggerEventType[actualCard.Triggered.EventType].Remove(card.Id)
                    )
                };
            }

            game = game with { CardsById = CardsById.Remove(card.Id) };

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                return game.RecalculateMultipliers();
            }
            else
            {
                return game;
            }
        }

        /// <summary>
        /// Gets a modified state that includes the passed-in <see cref="Location"/> as appropriate.
        /// </summary>
        private Game WithLocation(Location location)
        {
            return location.Column switch
            {
                Column.Left => this with { Left = location },
                Column.Middle => this with { Middle = location },
                Column.Right => this with { Right = location },
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Updates collections with any necessary changes when something is different about a card.
        ///
        /// Must be called whenever a card is modified, played, moved, removed, etc.
        /// </summary>
        private Game WithUpdatedCards(params ICardInstance[] cards)
        {
            var cardEventTriggers = CardEventTriggers;
            var cardsByTriggerEventType = CardsByTriggerEventType;
            var cardsById = CardsById;

            foreach (var card in cards)
            {
                var oldCard = CardsById.GetValueOrDefault(card.Id);
                var triggerApplied = oldCard?.Triggered?.AppliesInState(oldCard.State) ?? false;
                var triggerNowApplies = card.Triggered?.AppliesInState(card.State) ?? false;

                if (
                    oldCard?.Triggered?.EventType != card.Triggered?.EventType
                    || triggerApplied != triggerNowApplies
                )
                {
                    if (oldCard?.Triggered?.EventType != null && triggerApplied)
                    {
                        var cardsTriggered = cardsByTriggerEventType[oldCard.Triggered.EventType];
                        cardsTriggered = cardsTriggered.Remove(oldCard.Id);
                        cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                            oldCard.Triggered.EventType,
                            cardsTriggered
                        );

                        cardEventTriggers = cardEventTriggers.Remove(card.Id);
                    }

                    if (card.Triggered?.EventType != null && triggerNowApplies)
                    {
                        cardEventTriggers = cardEventTriggers.SetItem(
                            card.Id,
                            card.Triggered.EventType
                        );

                        var cardsTriggered =
                            cardsByTriggerEventType.GetValueOrDefault(card.Triggered.EventType)
                            ?? ImmutableHashSet<long>.Empty;
                        cardsTriggered = cardsTriggered.Add(card.Id);
                        cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                            card.Triggered.EventType,
                            cardsTriggered
                        );
                    }
                }

                cardsById = cardsById.SetItem(card.Id, card);
            }

            return this with
            {
                CardsById = cardsById,
                CardEventTriggers = cardEventTriggers,
                CardsByTriggerEventType = cardsByTriggerEventType
            };
        }

        /// <summary>
        /// Gets a modified state that includes the passed-in <see cref="Player"/> as appropriate.
        /// </summary>
        private Game WithPlayer(Player player)
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

            if (
                topPlayerActions.OfType<PlayCardAction>().Sum(pca => pca.Card.Cost)
                > this.Top.Energy
            )
            {
                throw new InvalidOperationException();
            }

            if (
                bottomPlayerActions.OfType<PlayCardAction>().Sum(pca => pca.Card.Cost)
                > this.Bottom.Energy
            )
            {
                throw new InvalidOperationException();
            }

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
            var actualCard = this.GetCard(card.Id);
            actualCard = actualCard.InPlayAt(actualCard.Column) with { State = CardState.InPlay };
            var game = this.WithModifiedCard(actualCard);

            game = game.TriggerOnRevealAbility(card);

            // This is to ensure that cards that get modified by their own reveal
            // abilities get attached to the reveal event in their modified state,
            // which may or may not be super useful at this point.
            //
            // The null coalesce operator is because of Hulkbuster.
            var revealedCard = game.AllCards.SingleOrDefault(c => c.Id == card.Id) ?? actualCard;

            game = game.WithEvent(new CardRevealedEvent(game.Turn, revealedCard));

            if (
                revealedCard.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || revealedCard.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || revealedCard.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || revealedCard.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                game = game.RecalculateMultipliers();
            }

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

            //var sensors = AllSensors;

            // TODO: Determine if we need to stack-order events for triggers, any other ordering constraints
            //var allCards = AllCards.ToList();

            if (CardsByTriggerEventType.TryGetValue(nextEvent.Type, out var cardIdsWithTrigger))
            {
                foreach (var cardId in cardIdsWithTrigger)
                {
                    var card = GetCardInstance(cardId);
                    game = card.Triggered.ProcessEvent(game, nextEvent, card);
                }
            }
            //foreach (var cardWithTrigger in allCards)
            //{
            //    if (cardWithTrigger.Triggered != null)
            //    {
            //        game = cardWithTrigger.Triggered.ProcessEvent(game, nextEvent, cardWithTrigger);
            //    }
            //}

            //foreach (var discardedOrDestroyedCard in originalState.Top.Discards)
            //{
            //    if (discardedOrDestroyedCard.Triggered?.DiscardedOrDestroyed() ?? false)
            //    {
            //        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
            //            game,
            //            nextEvent,
            //            discardedOrDestroyedCard
            //        );
            //    }
            //}

            //foreach (var discardedOrDestroyedCard in originalState.Top.Destroyed)
            //{
            //    if (discardedOrDestroyedCard.Triggered != null)
            //    {
            //        if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed())
            //        {
            //            game = discardedOrDestroyedCard.Triggered.ProcessEvent(
            //                game,
            //                nextEvent,
            //                discardedOrDestroyedCard
            //            );
            //        }
            //    }
            //}

            //foreach (var discardedOrDestroyedCard in originalState.Bottom.Discards)
            //{
            //    if (discardedOrDestroyedCard.Triggered?.DiscardedOrDestroyed() ?? false)
            //    {
            //        game = discardedOrDestroyedCard.Triggered.ProcessEvent(
            //            game,
            //            nextEvent,
            //            discardedOrDestroyedCard
            //        );
            //    }
            //}

            //foreach (var discardedOrDestroyedCard in originalState.Bottom.Destroyed)
            //{
            //    if (discardedOrDestroyedCard.Triggered != null)
            //    {
            //        if (discardedOrDestroyedCard.Triggered.DiscardedOrDestroyed())
            //        {
            //            game = discardedOrDestroyedCard.Triggered.ProcessEvent(
            //                game,
            //                nextEvent,
            //                discardedOrDestroyedCard
            //            );
            //        }
            //    }
            //}

            //foreach (var cardInHand in originalState.Top.Hand)
            //{
            //    if (cardInHand.Triggered != null)
            //    {
            //        if (cardInHand.Triggered.InHand())
            //        {
            //            game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
            //        }
            //    }
            //}

            //foreach (var cardInHand in originalState.Bottom.Hand)
            //{
            //    if (cardInHand.Triggered != null)
            //    {
            //        if (cardInHand.Triggered.InHand())
            //        {
            //            game = cardInHand.Triggered.ProcessEvent(game, nextEvent, cardInHand);
            //        }
            //    }
            //}

            //foreach (var cardInLibrary in originalState.Top.Library)
            //{
            //    if (cardInLibrary.Triggered != null)
            //    {
            //        if (cardInLibrary.Triggered.InDeck())
            //        {
            //            game =
            //                cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary)
            //                ?? game;
            //        }
            //    }
            //}

            //foreach (var cardInLibrary in originalState.Bottom.Library)
            //{
            //    if (cardInLibrary.Triggered != null)
            //    {
            //        if (cardInLibrary.Triggered.InDeck())
            //        {
            //            game =
            //                cardInLibrary.Triggered?.ProcessEvent(game, nextEvent, cardInLibrary)
            //                ?? game;
            //        }
            //    }
            //}

            if (SensorsByTriggerEventType.TryGetValue(nextEvent.Type, out var sensorIdsWithTrigger))
            {
                foreach (var sensorId in sensorIdsWithTrigger)
                {
                    var sensor = GetSensor(sensorId);
                    game = sensor.TriggeredAbility.ProcessEvent(game, nextEvent, sensor);
                }
            }
            //foreach (var sensor in sensors)
            //{
            //    game = sensor.TriggeredAbility?.ProcessEvent(game, nextEvent, sensor) ?? game;
            //}

            foreach (var location in Locations)
            {
                if (location.Revealed && location.Definition.Triggered != null)
                {
                    game = location.Definition.Triggered.ProcessEvent(game, nextEvent, location);
                }
            }

            return game;
        }

        public Game PlayCardUnsafe(ICardInstance card, Column column)
        {
            var player = this[card.Side];
            player = player with
            {
                Energy = player.Energy - card.Cost,
                Hand = player.Hand.Remove(card)
            };

            var cardInPlay = card.InPlayAt(column) with { State = CardState.PlayedButNotRevealed };
            var location = this[column].WithCard(cardInPlay);

            return this.WithPlayer(player)
                .WithLocation(location)
                .WithUpdatedCards(cardInPlay)
                .WithEvent(new CardPlayedEvent(this.Turn, cardInPlay));
        }

        public Game MoveCardUnsafe(ICard card, Column to)
        {
            var fromLocation = this[card.Column];
            var toLocation = this[to];

            fromLocation = fromLocation.WithoutCard(card);
            card = card.InPlayAt(to);
            toLocation = toLocation.WithCard(card);

            var game = this.WithLocation(fromLocation)
                .WithLocation(toLocation)
                .WithUpdatedCards(card)
                .WithEvent(new CardMovedEvent(this.Turn, card, fromLocation.Column, to));

            if (
                card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects
                || card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals
                || card.Ongoing?.Type == OngoingAbilityType.BlockLocationEffects
                || card.Ongoing?.Type == OngoingAbilityType.BlockCardEffects
            )
            {
                game = game.RecalculateMultipliers();
            }

            return game;
        }

        /// <summary>
        /// Recalculates any multipliers for effects (e.g., Wong, Onslaught).
        /// </summary>
        public Game RecalculateMultipliers()
        {
            var allCards = AllCards.ToList();
            var cardsByColumn = AllCards
                .GroupBy(c => c.Column)
                .ToDictionary(g => g.Key, g => g.ToList());
            var effectBlockers = GetEffectBlockers(allCards);

            var game = this;

            foreach (var column in All.Columns)
            {
                var location = this[column];
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

                //var topCards = location.TopCards;
                //var bottomCards = location.BottomCards;

                // We have to figure out ongoing multipliers first, because Wong's ability
                // is itself an ongoing ability, and can therefore be multiplied

                if (cardsByColumn.ContainsKey(location.Column))
                {
                    var cardsInLocation = cardsByColumn[location.Column];

                    for (var i = 0; i < cardsInLocation.Count; i++)
                    {
                        var card = cardsInLocation[i];
                        if (card.Column == location.Column)
                        {
                            if (card.Ongoing?.Type == OngoingAbilityType.DoubleOngoingEffects)
                            {
                                if (card.Side == Side.Top)
                                {
                                    ongoingTop *= 2;
                                }
                                else
                                {
                                    ongoingBottom *= 2;
                                }
                            }
                        }
                    }

                    for (var i = 0; i < cardsInLocation.Count; i++)
                    {
                        var card = cardsInLocation[i];
                        if (card.Column == location.Column)
                        {
                            if (card.Ongoing?.Type == OngoingAbilityType.DoubleOnReveals)
                            {
                                if (card.Side == Side.Top)
                                {
                                    for (var j = 0; j < ongoingTop; j++)
                                    {
                                        onRevealTop *= 2;
                                    }
                                }
                                else
                                {
                                    for (var j = 0; j < ongoingBottom; j++)
                                    {
                                        onRevealBottom *= 2;
                                    }
                                }
                            }
                        }
                    }
                }

                //foreach (var card in topCards)
                //{
                //    if (card.State == CardState.InPlay)
                //    {
                //        if (card.Ongoing is OngoingDoubleOnReveal<ICard>)
                //        {
                //            for (var i = 0; i < ongoingTop; i++)
                //            {
                //                onRevealTop *= 2;
                //            }
                //        }
                //    }
                //}


                //foreach (var card in topCards)
                //{
                //    if (card.State == CardState.InPlay)
                //    {
                //        if (card.Ongoing is OngoingDoubleOtherOngoing<ICard>)
                //        {
                //            ongoingTop *= 2;
                //        }
                //    }
                //}

                //foreach (var card in bottomCards)
                //{
                //    if (card.State == CardState.InPlay)
                //    {
                //        if (card.Ongoing is OngoingDoubleOtherOngoing<ICard>)
                //        {
                //            ongoingBottom *= 2;
                //        }
                //    }
                //}

                //foreach (var card in topCards)
                //{
                //    if (card.State == CardState.InPlay)
                //    {
                //        if (card.Ongoing is OngoingDoubleOnReveal<ICard>)
                //        {
                //            for (var i = 0; i < ongoingTop; i++)
                //            {
                //                onRevealTop *= 2;
                //            }
                //        }
                //    }
                //}

                //foreach (var card in bottomCards)
                //{
                //    if (card.State == CardState.InPlay)
                //    {
                //        if (card.Ongoing is OngoingDoubleOnReveal<ICard>)
                //        {
                //            for (var i = 0; i < ongoingBottom; i++)
                //            {
                //                onRevealBottom *= 2;
                //            }
                //        }
                //    }
                //}

                var topMultipliers = new Multipliers(onRevealTop, ongoingTop);
                var bottomMultipliers = new Multipliers(onRevealBottom, ongoingBottom);

                var changed = false;
                if (topMultipliers != location.TopMultipliers)
                {
                    location = location with { TopMultipliers = topMultipliers };
                    changed = true;
                }

                if (bottomMultipliers != location.BottomMultipliers)
                {
                    location = location with { BottomMultipliers = bottomMultipliers };
                    changed = true;
                }

                if (changed)
                {
                    game = game.WithLocation(location);
                }
            }

            return game;
        }

        public Game RecalculatePower()
        {
            var recalculatedCards = new List<ICard>();

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

                foreach (var mod in card.Modifications)
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

                if (power != card.Power || powerAdjustment != card.PowerAdjustment)
                {
                    recalculatedCards.Add(
                        card.InPlayAt(card.Column) with
                        {
                            Power = power,
                            PowerAdjustment = powerAdjustment
                        }
                    );
                }
            }

            if (recalculatedCards.Count > 0)
            {
                return this.WithModifiedCards(recalculatedCards);
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

                    if (adjustPower.Selector.Selects(card, ongoing.Source, this))
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

                    if (adjustPower.Selector.Selects(card, ongoing.Source, this))
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
                    if (card.Ongoing?.Type == OngoingAbilityType.AddPowerToLocation)
                    {
                        var addLocationPower = card.Ongoing as OngoingAdjustLocationPower<ICard>;
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

        #region Other Helpers

        // TODO: See if we can make this more automatic
        /// <summary>
        /// Helper function for anything that can't happen automatically within the constructor.
        ///
        /// MUST be called exactly once, right after the constructor.
        /// </summary>
        public Game Initialize()
        {
            return this.WithUpdatedCards(Top.Library.Cards.Concat(Bottom.Library.Cards).ToArray());
        }

        #endregion
    }
}
