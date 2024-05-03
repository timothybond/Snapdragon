using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Snapdragon.GameAccessors
{
    /// <summary>
    /// An object that stores some core details of the game state.
    ///
    /// Specifically, it tracks the location and status of all cards.
    ///
    /// The base constructor should not be used directly.
    /// </summary>
    public record GameKernel(
        int Turn,
        LocationDefinition LeftLocationDefinition,
        LocationDefinition MiddleLocationDefinition,
        LocationDefinition RightLocationDefinition,
        ImmutableDictionary<long, CardBase> Cards,
        ImmutableDictionary<long, Side> CardSides,
        ImmutableDictionary<long, CardState> CardStates,
        ImmutableDictionary<long, Column?> CardLocations,
        ImmutableHashSet<long> PlayedCards,
        ImmutableHashSet<long> RevealedCards,
        ImmutableList<long> TopLeftCards,
        ImmutableList<long> TopMiddleCards,
        ImmutableList<long> TopRightCards,
        ImmutableList<long> BottomLeftCards,
        ImmutableList<long> BottomMiddleCards,
        ImmutableList<long> BottomRightCards,
        ImmutableList<long> TopHand,
        ImmutableList<long> TopLibrary,
        ImmutableList<long> TopDiscards,
        ImmutableList<long> TopDestroyed,
        ImmutableList<long> BottomHand,
        ImmutableList<long> BottomLibrary,
        ImmutableList<long> BottomDiscards,
        ImmutableList<long> BottomDestroyed,
        ImmutableDictionary<long, Sensor<ICard>> Sensors,
        ImmutableDictionary<long, Side> SensorSides,
        ImmutableDictionary<long, Column> SensorLocations,
        ImmutableList<long> TopLeftSensors,
        ImmutableList<long> TopMiddleSensors,
        ImmutableList<long> TopRightSensors,
        ImmutableList<long> BottomLeftSensors,
        ImmutableList<long> BottomMiddleSensors,
        ImmutableList<long> BottomRightSensors,
        bool LeftRevealed,
        bool MiddleRevealed,
        bool RightRevealed,
        Multipliers TopLeftMultipliers,
        Multipliers TopMiddleMultipliers,
        Multipliers TopRightMultipliers,
        Multipliers BottomLeftMultipliers,
        Multipliers BottomMiddleMultipliers,
        Multipliers BottomRightMultipliers,
        ImmutableDictionary<long, EventType> CardEventTriggers,
        ImmutableDictionary<EventType, ImmutableHashSet<long>> CardsByTriggerEventType,
        ImmutableDictionary<long, EventType> SensorEventTriggers,
        ImmutableDictionary<EventType, ImmutableHashSet<long>> SensorsByTriggerEventType
    )
    {
        public static GameKernel FromPlayersAndLocations(
            Player top,
            Player bottom,
            LocationDefinition left,
            LocationDefinition middle,
            LocationDefinition right
        )
        {
            var topCards = top.Configuration.Deck.Cards.Select(c => new CardBase(c)).ToList();
            var bottomCards = bottom.Configuration.Deck.Cards.Select(c => new CardBase(c)).ToList();

            var cards = topCards.Concat(bottomCards).ToImmutableDictionary(c => c.Id);
            var cardSides = topCards
                .Select(c => (c.Id, Side.Top))
                .Concat(bottomCards.Select(c => (c.Id, Side.Bottom)))
                .ToImmutableDictionary(kp => kp.Id, kp => kp.Item2);

            var cardStates = cards.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => CardState.InLibrary
            );
            var cardLocations = cards.ToImmutableDictionary(kvp => kvp.Key, kvp => (Column?)null);

            // Initially the only triggers possible are those that happen when in the library
            var cardEventTriggers = cards
                .Values.Where(c => c.Triggered?.WhenInDeck ?? false)
                .ToImmutableDictionary(c => c.Id, c => c.Triggered!.EventType);

            var cardsByEventType = cardEventTriggers
                .GroupBy(kvp => kvp.Value)
                .ToImmutableDictionary(
                    g => g.Key,
                    g => g.Select(kvp => kvp.Key).ToImmutableHashSet()
                );

            return new GameKernel(
                0,
                left,
                middle,
                right,
                cards,
                cardSides,
                cardStates,
                cardLocations,
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                topCards.Select(c => c.Id).ToImmutableList(),
                [],
                [],
                [],
                bottomCards.Select(c => c.Id).ToImmutableList(),
                [],
                [],
                ImmutableDictionary<long, Sensor<ICard>>.Empty,
                ImmutableDictionary<long, Side>.Empty,
                ImmutableDictionary<long, Column>.Empty,
                [],
                [],
                [],
                [],
                [],
                [],
                false,
                false,
                false,
                new(),
                new(),
                new(),
                new(),
                new(),
                new(),
                cardEventTriggers,
                cardsByEventType,
                ImmutableDictionary<long, EventType>.Empty,
                ImmutableDictionary<EventType, ImmutableHashSet<long>>.Empty
            );
        }

        public IReadOnlyList<ICard> this[Column column, Side side] =>
            new CardInLocationListReference(this, CardIdsForLocation(column, side));

        public IReadOnlyList<ICard> AllCards =>
            new CardInLocationListReference(this, RevealedCards.ToList());

        public IReadOnlyList<ICard> AllCardsIncludingUnrevealed =>
            new CardInLocationListReference(this, PlayedCards.ToList());

        public Location this[Column column]
        {
            get
            {
                switch (column)
                {
                    case Column.Left:
                        return new Location(
                            Column.Left,
                            LeftLocationDefinition,
                            this,
                            TopLeftMultipliers,
                            BottomLeftMultipliers
                        );
                    case Column.Middle:
                        return new Location(
                            Column.Middle,
                            MiddleLocationDefinition,
                            this,
                            TopMiddleMultipliers,
                            BottomMiddleMultipliers
                        );
                    case Column.Right:
                        return new Location(
                            Column.Right,
                            RightLocationDefinition,
                            this,
                            TopRightMultipliers,
                            BottomRightMultipliers
                        );
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public ICardInstance? this[long cardId]
        {
            get
            {
                var card = Cards.GetValueOrDefault(cardId);
                if (card == null)
                {
                    return null;
                }

                if (
                    CardStates[cardId] == CardState.InPlay
                    || CardStates[cardId] == CardState.PlayedButNotRevealed
                )
                {
                    return new Card(card, this);
                }

                return new CardInstance(card, this);
            }
        }

        #region Public Methods

        /// <summary>
        /// Draws a card for the given player.
        /// </summary>
        /// <param name="side">Side of the player to draw.</param>
        public GameKernel DrawCard(Side side)
        {
            long cardId;

            switch (side)
            {
                case Side.Top:
                    if (TopLibrary.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Cannot draw; top player library is empty."
                        );
                    }

                    cardId = TopLibrary[0];

                    return RemoveCardFromLibraryUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);

                case Side.Bottom:
                    if (BottomLibrary.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Cannot draw; bottom player library is empty."
                        );
                    }

                    cardId = BottomLibrary[0];

                    return RemoveCardFromLibraryUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Draws a card for the given player, from their opponent's deck.
        /// </summary>
        /// <param name="side">Side of the player to draw a card (not the side the card is drawn from).</param>
        public GameKernel DrawOpponentCard(Side side)
        {
            long cardId;

            switch (side)
            {
                case Side.Top:
                    if (BottomLibrary.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Cannot draw; top player library is empty."
                        );
                    }

                    cardId = BottomLibrary[0];

                    return RemoveCardFromLibraryUnsafe(cardId, side.Other())
                        .SetCardSideUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);

                case Side.Bottom:
                    if (TopLibrary.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Cannot draw; bottom player library is empty."
                        );
                    }

                    cardId = TopLibrary[0];

                    return RemoveCardFromLibraryUnsafe(cardId, side.Other())
                        .SetCardSideUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates the stored record for a card.
        /// </summary>
        /// <param name="card">The new card state to store.</param>
        public GameKernel WithUpdatedCard(ICardInstance card)
        {
            return this with { Cards = Cards.SetItem(card.Id, card.Base) };
        }

        /// <summary>
        /// Updates the stored record for a card.
        /// </summary>
        /// <param name="card">The new card state to store.</param>
        public GameKernel WithUpdatedCard(CardBase cardBase)
        {
            return this with { Cards = Cards.SetItem(cardBase.Id, cardBase) };
        }

        /// <summary>
        /// Plays a card into a specific location and side.
        ///
        /// That card must:
        /// - Exist within the kernel
        /// - Be in its owner's hand
        /// - Not have a location (which should be true based on the above checks)
        ///
        /// It will be removed from its owner's hand,
        /// given the state <see cref="CardState.PlayedButNotRevealed"/>,
        /// and assigned to the specified location.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to place.</param>
        /// <param name="column">Destination location.</param>
        /// <param name="side">Destination side.</param>
        public GameKernel PlayCard(long cardId, Column column, Side side)
        {
            if (!Cards.ContainsKey(cardId))
            {
                throw new InvalidOperationException($"Unknown card {cardId}.");
            }

            var card = Cards[cardId];

            if (CardStates[cardId] != CardState.InHand)
            {
                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]}, not {CardState.InHand}."
                );
            }

            if (CardLocations[cardId] != null)
            {
                throw new UnreachableException(
                    $"Invoked {nameof(PlayCard)} for card {card.Name} ({card.Id}) which is already at location {CardLocations[cardId]}."
                );
            }

            var newCollection = CardIdsForLocation(column, side);

            if (newCollection.Count + 1 > Max.CardsPerLocation)
            {
                // Technically redundant
                throw new InvalidOperationException(
                    $"Attempted to place {newCollection.Count + 1} items at {column}, {side} but max is {Max.CardsPerLocation}."
                );
            }

            newCollection = newCollection.Add(cardId);

            var newKernel = this with { PlayedCards = PlayedCards.Add(cardId) };

            return newKernel
                .RemoveCardFromHandUnsafe(cardId, side)
                .WithCardInStateUnsafe(cardId, CardState.PlayedButNotRevealed)
                .WithUpdatedLocationCollection(column, side, newCollection);
        }

        /// <summary>
        /// Moves a card from one column to another.
        ///
        /// This requires that the card be in state <see cref="CardState.InPlay"/>.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to move.</param>
        /// <param name="side">Side of the card.</param>
        /// <param name="from">Current location of the card.</param>
        /// <param name="to">New location of the card.</param>
        /// <returns></returns>
        public GameKernel MoveCard(long cardId, Side side, Column from, Column to)
        {
            ValidateCardExists(cardId);
            ValidateState(cardId, CardState.InPlay, CardState.PlayedButNotRevealed);
            ValidateLocation(cardId, from);

            return RemoveCardFromLocationUnsafe(cardId, from, side)
                .AddCardToLocationUnsafe(cardId, to, side);
        }

        /// <summary>
        /// Updates a played-but-unrevealed card to be revealed.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        public GameKernel RevealCard(long cardId)
        {
            if (!Cards.ContainsKey(cardId))
            {
                throw new InvalidOperationException($"Unknown card {cardId}.");
            }

            var card = Cards[cardId];

            if (CardStates[cardId] != CardState.PlayedButNotRevealed)
            {
                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]}, not {CardState.PlayedButNotRevealed}."
                );
            }

            if (CardLocations[cardId] == null)
            {
                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]} but has no location stored."
                );
            }

            return (this with { RevealedCards = RevealedCards.Add(cardId) })
                .WithCardInStateUnsafe(cardId, CardState.InPlay)
                .WithUpdatedCard(card with { TurnRevealed = Turn });
        }

        /// <summary>
        /// Discards a card from the given player's hand.
        ///
        /// This will add it to the player's discarded cards collection.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to discard.</param>
        /// <param name="side">Player to discard.</param>
        public GameKernel DiscardCard(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateState(cardId, CardState.InHand);
            ValidateSide(cardId, side);

            switch (side)
            {
                case Side.Top:
                    return RemoveCardFromHandUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.Discarded) with
                    {
                        TopDiscards = TopDiscards.Add(cardId)
                    };
                case Side.Bottom:
                    return RemoveCardFromHandUnsafe(cardId, side)
                        .WithCardInStateUnsafe(cardId, CardState.Discarded) with
                    {
                        BottomDiscards = BottomDiscards.Add(cardId)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Destroys an in-play card.
        ///
        /// This will add it to the player's destroyed cards collection.
        /// </summary>
        /// <param name="cardId">Unique identifier of card to destroy.</param>
        /// <param name="column">Location of the card.</param>
        /// <param name="side">Side of the card.</param>
        public GameKernel DestroyCardFromPlay(long cardId, Column column, Side side)
        {
            ValidateCardExists(cardId);
            ValidateState(cardId, CardState.InPlay, CardState.PlayedButNotRevealed);
            ValidateLocation(cardId, column);
            ValidateSide(cardId, side);

            var kernel = RemoveCardFromLocationUnsafe(cardId, column, side)
                .WithCardInStateUnsafe(cardId, CardState.Destroyed) with
            {
                PlayedCards = PlayedCards.Remove(cardId),
                RevealedCards = RevealedCards.Remove(cardId)
            };

            switch (side)
            {
                case Side.Top:
                    return kernel with { TopDestroyed = TopDestroyed.Add(cardId) };
                case Side.Bottom:
                    return kernel with { BottomDestroyed = BottomDestroyed.Add(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Destroys a card in a player's hand.
        ///
        /// This will effectively remove the card from the game.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="side">Side of the card.</param>
        public GameKernel DestroyCardFromHand(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateState(cardId, CardState.InHand);
            ValidateSide(cardId, side);

            return RemoveCardFromGame(cardId);
        }

        /// <summary>
        /// Destroys a card in a player's library.
        ///
        /// This will effectively remove the card from the game.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="side">Side of the card.</param>
        public GameKernel DestroyCardFromLibrary(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateState(cardId, CardState.InLibrary);
            ValidateSide(cardId, side);

            return RemoveCardFromGame(cardId);
        }

        /// <summary>
        /// Returns a card to the owner's hand, either from a location
        /// in play or from the discards/destroyed collection.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="side">Card owner's side.</param>
        public GameKernel ReturnCardToHand(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);

            var kernel = this;
            var card = Cards[cardId];

            switch (CardStates[cardId])
            {
                case CardState.InHand:
                    throw new InvalidOperationException(
                        $"Card {card.Name} ({card.Id}) is already in the owner's hand."
                    );
                case CardState.InLibrary:
                    throw new InvalidOperationException(
                        $"Card {card.Name} ({card.Id}) is in the owner's library."
                    );
                case CardState.InPlay:
                case CardState.PlayedButNotRevealed:
                    var column = CardLocations[cardId];
                    if (column == null)
                    {
                        throw new InvalidOperationException(
                            $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]} but has no location."
                        );
                    }
                    kernel = kernel.RemoveCardFromLocationUnsafe(cardId, column.Value, side) with
                    {
                        PlayedCards = PlayedCards.Remove(cardId),
                        RevealedCards = RevealedCards.Remove(cardId)
                    };
                    break;
                case CardState.Discarded:
                    kernel = kernel.RemoveCardFromDiscardsUnsafe(cardId, side);
                    break;
                case CardState.Destroyed:
                    kernel = kernel.RemoveCardFromDestroyedUnsafe(cardId, side);
                    break;
                default:
                    throw new NotImplementedException();
            }

            kernel = kernel
                .WithCardInStateUnsafe(cardId, CardState.InHand)
                .AddCardToHandUnsafe(cardId, side);

            return kernel;
        }

        /// <summary>
        /// Switches the side of a card. It must be in play, and will be moved to the appropriate location collection.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="currentSide">Current side of the card (not the one it will be switched to).</param>
        public GameKernel SwitchCardSide(long cardId, Side currentSide)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, currentSide);
            ValidateState(cardId, CardState.InPlay, CardState.PlayedButNotRevealed);

            var card = Cards[cardId];

            var column =
                CardLocations[cardId]
                ?? throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]} but has no location."
                );

            return RemoveCardFromLocationUnsafe(cardId, column, currentSide)
                .SetCardSideUnsafe(cardId, currentSide.Other())
                .AddCardToLocationUnsafe(cardId, column, currentSide.Other());
        }

        /// <summary>
        /// Returns a discarded card to the given location.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="column">Destination location.</param>
        /// <param name="side">Side of the card.</param>
        public GameKernel ReturnDiscardToLocation(long cardId, Column column, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.Discarded);

            return RemoveCardFromDiscardsUnsafe(cardId, side)
                .WithCardInStateUnsafe(cardId, CardState.InPlay)
                .AddCardToLocationUnsafe(cardId, column, side) with
            {
                PlayedCards = PlayedCards.Add(cardId),
                RevealedCards = RevealedCards.Add(cardId)
            };
        }

        /// <summary>
        /// Returns a destroyed card to the given location.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="column">Destination location.</param>
        /// <param name="side">Side of the card.</param>
        public GameKernel ReturnDestroyedToLocation(long cardId, Column column, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.Destroyed);

            return RemoveCardFromDestroyedUnsafe(cardId, side)
                .WithCardInStateUnsafe(cardId, CardState.InPlay)
                .AddCardToLocationUnsafe(cardId, column, side) with
            {
                PlayedCards = PlayedCards.Add(cardId),
                RevealedCards = RevealedCards.Add(cardId)
            };
        }

        /// <summary>
        /// Adds a new card to the game and to the player's hand.
        /// </summary>
        /// <param name="cardDefinition">Definition of the card to add.</param>
        /// <param name="side">Player who will get the added card.</param>
        /// <param name="newCardId">Generated unique identifier of the added card.</param>
        public GameKernel AddNewCardToHand(
            CardDefinition cardDefinition,
            Side side,
            out long newCardId
        )
        {
            var cardBase = new CardBase(cardDefinition);

            newCardId = cardBase.Id;

            return WithNewCardUnsafe(cardBase, side, CardState.InHand)
                .AddCardToHandUnsafe(cardBase.Id, side);
        }

        /// <summary>
        /// Adds a new card to the game and to the player's library.
        /// </summary>
        /// <param name="cardDefinition">Definition of the card to add.</param>
        /// <param name="side">Player who will get the added card.</param>
        /// <param name="newCardId">Generated unique identifier of the added card.</param>
        public GameKernel AddNewCardToLibrary(
            CardDefinition cardDefinition,
            Side side,
            out long newCardId
        )
        {
            var cardBase = new CardBase(cardDefinition);

            newCardId = cardBase.Id;

            return WithNewCardUnsafe(cardBase, side, CardState.InLibrary)
                .AddCardToLibraryUnsafe(cardBase.Id, side);
        }

        /// <summary>
        /// Adds a new card to the game and to a specific location.
        /// </summary>
        /// <param name="cardDefinition">Definition of the card to add.</param>
        /// <param name="column">Location of the added card.</param>
        /// <param name="side">Player who will get the added card.</param>
        /// <param name="newCardId">Generated unique identifier of the added card.</param>
        public GameKernel AddNewCardToLocation(
            CardDefinition cardDefinition,
            Column column,
            Side side,
            out long newCardId
        )
        {
            var cardBase = new CardBase(cardDefinition);

            newCardId = cardBase.Id;

            return WithNewCardUnsafe(cardBase, side, CardState.InPlay)
                .AddCardToLocationUnsafe(cardBase.Id, column, side) with
            {
                PlayedCards = PlayedCards.Add(newCardId),
                RevealedCards = RevealedCards.Add(newCardId)
            };
        }

        /// <summary>
        /// Adds a copy of an existing card to the specified player's hand.
        /// </summary>
        /// <param name="existingCardId">Unique identifier of the existing card to copy.</param>
        /// <param name="side">Player who will get the added card.</param>
        /// <param name="newCardId">Generated unique identifier of the copy.</param>
        public GameKernel AddCopiedCardToHand(long existingCardId, Side side, out long newCardId)
        {
            ValidateCardExists(existingCardId);

            var copiedCard = Cards[existingCardId] with { Id = Ids.GetNextCard() };
            newCardId = copiedCard.Id;

            return WithNewCardUnsafe(copiedCard, side, CardState.InHand)
                .AddCardToHandUnsafe(copiedCard.Id, side);
        }

        /// <summary>
        /// Adds a copy of an existing card to the game and to a specific location.
        /// </summary>
        /// <param name="existingCardId">Unique identifier of the existing card to copy.</param>
        /// <param name="column">Location of the added card.</param>
        /// <param name="side">Player who will get the added card.</param>
        /// <param name="newCardId">Generated unique identifier of the copy.</param>
        public GameKernel AddCopiedCardToLocation(
            long existingCardId,
            Column column,
            Side side,
            out long newCardId
        )
        {
            ValidateCardExists(existingCardId);

            var copiedCard = Cards[existingCardId] with { Id = Ids.GetNextCard() };
            newCardId = copiedCard.Id;

            return WithNewCardUnsafe(copiedCard, side, CardState.InPlay)
                .AddCardToLocationUnsafe(copiedCard.Id, column, side) with
            {
                PlayedCards = PlayedCards.Add(newCardId),
                RevealedCards = RevealedCards.Add(newCardId)
            };
        }

        public GameKernel RevealLocation(Column column)
        {
            // TODO: Consider throwing if it's already revealed
            switch (column)
            {
                case Column.Left:
                    return this with { LeftRevealed = true };
                case Column.Middle:
                    return this with { MiddleRevealed = true };
                case Column.Right:
                    return this with { RightRevealed = true };
                default:
                    throw new NotImplementedException();
            }
        }

        public GameKernel AddSensor(Sensor<ICard> sensor)
        {
            if (Sensors.ContainsKey(sensor.Id))
            {
                throw new InvalidOperationException($"Sensor {sensor.Id} already exists.");
            }

            var topLeftSensors = TopLeftSensors;
            var topMiddleSensors = TopMiddleSensors;
            var topRightSensors = TopRightSensors;
            var bottomLeftSensors = BottomLeftSensors;
            var bottomMiddleSensors = BottomMiddleSensors;
            var bottomRightSensors = BottomRightSensors;

            // TODO: Make a more succinct implementation
            switch (sensor.Column)
            {
                case Column.Left:
                    switch (sensor.Side)
                    {
                        case Side.Top:
                            topLeftSensors = topLeftSensors.Add(sensor.Id);
                            break;
                        case Side.Bottom:
                            bottomLeftSensors = bottomLeftSensors.Add(sensor.Id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case Column.Middle:
                    switch (sensor.Side)
                    {
                        case Side.Top:
                            topMiddleSensors = topMiddleSensors.Add(sensor.Id);
                            break;
                        case Side.Bottom:
                            bottomMiddleSensors = bottomMiddleSensors.Add(sensor.Id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case Column.Right:
                    switch (sensor.Side)
                    {
                        case Side.Top:
                            topRightSensors = topRightSensors.Add(sensor.Id);
                            break;
                        case Side.Bottom:
                            bottomRightSensors = bottomRightSensors.Add(sensor.Id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            var sensorEventTriggers = SensorEventTriggers;
            var sensorsByTriggerEventType = SensorsByTriggerEventType;

            if (sensor.TriggeredAbility?.EventType != null)
            {
                var eventType = sensor.TriggeredAbility.EventType;

                sensorEventTriggers = sensorEventTriggers.Add(sensor.Id, eventType);

                if (
                    sensorsByTriggerEventType.TryGetValue(
                        eventType,
                        out ImmutableHashSet<long> existingSensors
                    )
                )
                {
                    sensorsByTriggerEventType = sensorsByTriggerEventType.SetItem(
                        eventType,
                        existingSensors.Add(sensor.Id)
                    );
                }
                else
                {
                    sensorsByTriggerEventType = sensorsByTriggerEventType.SetItem(
                        eventType,
                        [sensor.Id]
                    );
                }
            }

            return this with
            {
                Sensors = Sensors.Add(sensor.Id, sensor),
                SensorSides = SensorSides.Add(sensor.Id, sensor.Side),
                SensorLocations = SensorLocations.Add(sensor.Id, sensor.Column),
                TopLeftSensors = topLeftSensors,
                TopMiddleSensors = topMiddleSensors,
                TopRightSensors = topRightSensors,
                BottomLeftSensors = bottomLeftSensors,
                BottomMiddleSensors = bottomMiddleSensors,
                BottomRightSensors = bottomRightSensors,
                SensorEventTriggers = sensorEventTriggers,
                SensorsByTriggerEventType = sensorsByTriggerEventType
            };
        }

        public GameKernel DestroySensor(long sensorId, Column column, Side side)
        {
            var sensorEventTriggers = SensorEventTriggers;
            var sensorsByTriggerEventType = SensorsByTriggerEventType;

            if (sensorEventTriggers.TryGetValue(sensorId, out EventType eventType))
            {
                sensorEventTriggers = sensorEventTriggers.Remove(sensorId);
                sensorsByTriggerEventType = sensorsByTriggerEventType.SetItem(
                    eventType,
                    sensorsByTriggerEventType[eventType].Remove(sensorId)
                );
            }

            return RemoveSensorFromLocationUnsafe(sensorId, column, side) with
            {
                Sensors = Sensors.Remove(sensorId),
                SensorSides = SensorSides.Remove(sensorId),
                SensorLocations = SensorLocations.Remove(sensorId),
                TopLeftSensors = TopLeftSensors.Remove(sensorId),
                TopMiddleSensors = TopMiddleSensors.Remove(sensorId),
                TopRightSensors = TopRightSensors.Remove(sensorId),
                BottomLeftSensors = BottomLeftSensors.Remove(sensorId),
                BottomMiddleSensors = BottomMiddleSensors.Remove(sensorId),
                BottomRightSensors = BottomRightSensors.Remove(sensorId),
                SensorEventTriggers = sensorEventTriggers,
                SensorsByTriggerEventType = sensorsByTriggerEventType
            };
        }

        /// <summary>
        /// Removes a card from the game entirely.
        ///
        /// This is used for cases where items are destroyed from the hand or deck,
        /// in which case I believe they are unrecoverable, as well as cases where
        /// one card merges with another.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        public GameKernel RemoveCardFromGame(long cardId)
        {
            var cardEventTriggers = CardEventTriggers;
            var cardsByTriggerEventType = CardsByTriggerEventType;

            if (cardEventTriggers.TryGetValue(cardId, out EventType eventType))
            {
                cardEventTriggers = cardEventTriggers.Remove(cardId);
                cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                    eventType,
                    cardsByTriggerEventType[eventType].Remove(cardId)
                );
            }

            return this with
            {
                Cards = Cards.Remove(cardId),
                CardStates = CardStates.Remove(cardId),
                CardLocations = CardLocations.Remove(cardId),
                CardSides = CardSides.Remove(cardId),
                TopLeftCards = TopLeftCards.Remove(cardId),
                TopMiddleCards = TopMiddleCards.Remove(cardId),
                TopRightCards = TopRightCards.Remove(cardId),
                BottomLeftCards = BottomLeftCards.Remove(cardId),
                BottomMiddleCards = BottomMiddleCards.Remove(cardId),
                BottomRightCards = BottomRightCards.Remove(cardId),
                TopHand = TopHand.Remove(cardId),
                TopLibrary = TopLibrary.Remove(cardId),
                TopDiscards = TopDiscards.Remove(cardId),
                TopDestroyed = TopDestroyed.Remove(cardId),
                BottomHand = BottomHand.Remove(cardId),
                BottomLibrary = BottomLibrary.Remove(cardId),
                BottomDiscards = BottomDiscards.Remove(cardId),
                BottomDestroyed = BottomDestroyed.Remove(cardId),
                PlayedCards = PlayedCards.Remove(cardId),
                RevealedCards = RevealedCards.Remove(cardId),
                CardEventTriggers = cardEventTriggers,
                CardsByTriggerEventType = cardsByTriggerEventType
            };
        }

        public GameKernel UpdateMultipliers(Column column, Side side, Multipliers multipliers)
        {
            return (column, side) switch
            {
                (Column.Left, Side.Top) => this with { TopLeftMultipliers = multipliers },
                (Column.Middle, Side.Top) => this with { TopMiddleMultipliers = multipliers },
                (Column.Right, Side.Top) => this with { TopRightMultipliers = multipliers },
                (Column.Left, Side.Bottom) => this with { BottomLeftMultipliers = multipliers },
                (Column.Middle, Side.Bottom) => this with { BottomMiddleMultipliers = multipliers },
                (Column.Right, Side.Bottom) => this with { BottomRightMultipliers = multipliers },
                (_, _) => throw new NotImplementedException()
            };
        }

        #endregion

        #region Internal Logic


        /// <summary>
        /// Method to retrieve a card as an <see cref="ICard"/>.
        ///
        /// Assumes that the card is in fact valid and in play.
        /// </summary>
        private ICard GetCardUnsafe(long cardId)
        {
            var card = Cards.GetValueOrDefault(cardId);
            return new Card(card, this);
        }

        /// <summary>
        /// Internal helper function that removes a card from the
        /// Hand collection for the given player.
        ///
        /// Note this leaves the card in an invalid state and must
        /// be followed by logic to add the card somewhere else.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to remove.</param>
        /// <param name="side">Side of the card.</param>
        private GameKernel RemoveCardFromHandUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.InHand);

            switch (side)
            {
                case Side.Top:
                    return this with { TopHand = TopHand.Remove(cardId) };
                case Side.Bottom:
                    return this with { BottomHand = BottomHand.Remove(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Internal helper function that removes a card from the
        /// Library collection for the given player.
        ///
        /// Note this leaves the card in an invalid state and must
        /// be followed by logic to add the card somewhere else.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to remove.</param>
        /// <param name="side">Side of the card.</param>
        private GameKernel RemoveCardFromLibraryUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.InLibrary);

            switch (side)
            {
                case Side.Top:
                    return this with { TopLibrary = TopLibrary.Remove(cardId) };
                case Side.Bottom:
                    return this with { BottomLibrary = BottomLibrary.Remove(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Internal helper function that removes a card from the
        /// Discards collection for the given player.
        ///
        /// Note this leaves the card in an invalid state and must
        /// be followed by logic to add the card somewhere else.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to remove.</param>
        /// <param name="side">Side of the card.</param>
        private GameKernel RemoveCardFromDiscardsUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.Discarded);

            switch (side)
            {
                case Side.Top:
                    return this with { TopDiscards = TopDiscards.Remove(cardId) };
                case Side.Bottom:
                    return this with { BottomDiscards = BottomDiscards.Remove(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Internal helper function that removes a card from the
        /// Destroyed collection for the given player.
        ///
        /// Note this leaves the card in an invalid state and must
        /// be followed by logic to add the card somewhere else.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card to remove.</param>
        /// <param name="side">Side of the card.</param>
        private GameKernel RemoveCardFromDestroyedUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);
            ValidateState(cardId, CardState.Destroyed);

            switch (side)
            {
                case Side.Top:
                    return this with { TopDestroyed = TopDestroyed.Remove(cardId) };
                case Side.Bottom:
                    return this with { BottomDestroyed = BottomDestroyed.Remove(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Internal helper function that removes a card from a location.
        ///
        /// Should only be used as part of a larger operation as it does not add the card to any other collection.
        /// </summary>
        /// <param name="cardId">Unique id of the card to remove.</param>
        /// <param name="column">Current location of the card.</param>
        /// <param name="side">Current side of the card.</param>
        private GameKernel RemoveCardFromLocationUnsafe(long cardId, Column column, Side side)
        {
            ValidateCardExists(cardId);
            ValidateLocation(cardId, column);
            ValidateSide(cardId, side);

            return (column, side) switch
            {
                (Column.Left, Side.Top) => this with { TopLeftCards = TopLeftCards.Remove(cardId) },
                (Column.Middle, Side.Top)
                    => this with
                    {
                        TopMiddleCards = TopMiddleCards.Remove(cardId)
                    },
                (Column.Right, Side.Top)
                    => this with
                    {
                        TopRightCards = TopRightCards.Remove(cardId)
                    },
                (Column.Left, Side.Bottom)
                    => this with
                    {
                        BottomLeftCards = BottomLeftCards.Remove(cardId)
                    },
                (Column.Middle, Side.Bottom)
                    => this with
                    {
                        BottomMiddleCards = BottomMiddleCards.Remove(cardId)
                    },
                (Column.Right, Side.Bottom)
                    => this with
                    {
                        BottomRightCards = BottomRightCards.Remove(cardId)
                    },
                (_, _) => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Internal helper function that adds a card to a location.
        ///
        /// Should only be used as part of a larger operation as it
        /// does not remove the card from any other collection.
        /// </summary>
        /// <param name="cardId">Unique id of the card to add.</param>
        /// <param name="column">New location of the card.</param>
        /// <param name="side">New (and current) side of the card.</param>
        private GameKernel AddCardToLocationUnsafe(long cardId, Column column, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);

            var existingCards = CardIdsForLocation(column, side);

            return WithUpdatedLocationCollection(column, side, existingCards.Add(cardId));
        }

        /// <summary>
        /// Internal helper function that adds a card to a player's Hand.
        ///
        /// Should only be used as part of a larger operation as it
        /// does not remove the card from any other collection.
        /// </summary>
        /// <param name="cardId">Unique id of the card to add.</param>
        /// <param name="side">New (and current) side of the card.</param>
        private GameKernel AddCardToHandUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);

            switch (side)
            {
                case Side.Top:
                    return this with { TopHand = TopHand.Add(cardId) };
                case Side.Bottom:
                    return this with { BottomHand = BottomHand.Add(cardId) };
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Internal helper function that assigns a new value for the <see cref="Side"/> of a card.
        ///
        /// Should only be used as part of a larger operation as it does not add or remove
        /// the card to/from any collections.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="side">New side of the card.</param>
        /// <returns></returns>
        private GameKernel SetCardSideUnsafe(long cardId, Side side)
        {
            return this with { CardSides = CardSides.SetItem(cardId, side) };
        }

        /// <summary>
        /// Internal helper function that adds a card to a player's Library.
        ///
        /// Should only be used as part of a larger operation as it
        /// does not remove the card from any other collection.
        /// </summary>
        /// <param name="cardId">Unique id of the card to add.</param>
        /// <param name="side">New (and current) side of the card.</param>
        private GameKernel AddCardToLibraryUnsafe(long cardId, Side side)
        {
            ValidateCardExists(cardId);
            ValidateSide(cardId, side);

            switch (side)
            {
                case Side.Top:
                    return this with { TopLibrary = TopLibrary.Add(cardId) };
                case Side.Bottom:
                    return this with { BottomLibrary = BottomLibrary.Add(cardId) };
                default:
                    throw new NotImplementedException();
            }
            ;
        }

        /// <summary>
        /// Internal helper function that updates the card collection for a particular column and side.
        /// </summary>
        /// <param name="column">Location to update.</param>
        /// <param name="side">Side to update.</param>
        /// <param name="newLocationItems">New cards for that location and side.</param>
        private GameKernel WithUpdatedLocationCollection(
            Column column,
            Side side,
            ImmutableList<long> newLocationItems
        )
        {
            if (newLocationItems.Count > Max.CardsPerLocation)
            {
                throw new InvalidOperationException(
                    $"Attempted to place {newLocationItems.Count} items at {column}, {side} but max is {Max.CardsPerLocation}."
                );
            }

            var newCardLocationsBuilder = CardLocations.ToBuilder();

            foreach (var cardId in newLocationItems)
            {
                newCardLocationsBuilder[cardId] = column;
            }

            var newCardLocations = newCardLocationsBuilder.ToImmutable();

            return (column, side) switch
            {
                (Column.Left, Side.Top)
                    => this with
                    {
                        TopLeftCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (Column.Middle, Side.Top)
                    => this with
                    {
                        TopMiddleCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (Column.Right, Side.Top)
                    => this with
                    {
                        TopRightCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (Column.Left, Side.Bottom)
                    => this with
                    {
                        BottomLeftCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (Column.Middle, Side.Bottom)
                    => this with
                    {
                        BottomMiddleCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (Column.Right, Side.Bottom)
                    => this with
                    {
                        BottomRightCards = newLocationItems,
                        CardLocations = newCardLocations
                    },
                (_, _) => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Internal helper function that udates the state record of a card.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="state">New state to store for the card.</param>
        /// <returns></returns>
        private GameKernel WithCardInStateUnsafe(long cardId, CardState state)
        {
            var card = Cards[cardId];

            bool needsTrigger = NeedsTrigger(card, state);

            var cardEventTriggers = CardEventTriggers;
            var cardsByTriggerEventType = CardsByTriggerEventType;

            if (needsTrigger)
            {
                var eventType = card.Triggered.EventType;
                if (
                    !cardEventTriggers.TryGetValue(cardId, out EventType oldEventType)
                    || oldEventType == eventType
                )
                {
                    cardEventTriggers = cardEventTriggers.SetItem(cardId, eventType);

                    if (
                        cardsByTriggerEventType.TryGetValue(
                            eventType,
                            out ImmutableHashSet<long> existingCards
                        )
                    )
                    {
                        cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                            eventType,
                            existingCards.Add(cardId)
                        );
                    }
                    else
                    {
                        cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                            eventType,
                            [cardId]
                        );
                    }
                }
            }
            else // Need to remove existing trigger, if any
            {
                if (cardEventTriggers.TryGetValue(cardId, out EventType oldEventType))
                {
                    cardsByTriggerEventType = cardsByTriggerEventType.SetItem(
                        oldEventType,
                        cardsByTriggerEventType[oldEventType].Remove(cardId)
                    );
                    cardEventTriggers = cardEventTriggers.Remove(cardId);
                }
            }

            return this with
            {
                CardStates = CardStates.SetItem(cardId, state),
                CardEventTriggers = cardEventTriggers,
                CardsByTriggerEventType = cardsByTriggerEventType
            };
        }

        /// <summary>
        /// Internal helper function that adds a new card to the game,
        /// including putting it in the three universal collections,
        /// but no specific collection for its state/location.
        /// </summary>
        /// <param name="cardBase">New card to add.</param>
        private GameKernel WithNewCardUnsafe(CardBase cardBase, Side side, CardState initialState)
        {
            bool needsTrigger = NeedsTrigger(cardBase, initialState);

            var cardEventTriggers = CardEventTriggers;
            var cardsByTriggerEventType = CardsByTriggerEventType;

            if (needsTrigger)
            {
                var eventType = cardBase.Triggered.EventType;
                if (
                    !cardEventTriggers.TryGetValue(cardBase.Id, out EventType oldEventType)
                    || oldEventType == eventType
                )
                {
                    cardEventTriggers = cardEventTriggers.SetItem(cardBase.Id, eventType);

                    if (
                        cardsByTriggerEventType.TryGetValue(
                            eventType,
                            out ImmutableHashSet<long> existingCards
                        )
                    )
                    {
                        cardsByTriggerEventType.SetItem(eventType, existingCards.Add(cardBase.Id));
                    }
                    else
                    {
                        cardsByTriggerEventType.SetItem(eventType, [cardBase.Id]);
                    }
                }
            }

            return this with
            {
                Cards = Cards.Add(cardBase.Id, cardBase),
                CardSides = CardSides.Add(cardBase.Id, side),
                CardStates = CardStates.Add(cardBase.Id, initialState),
                CardLocations = CardLocations.Add(cardBase.Id, null),
                CardEventTriggers = cardEventTriggers,
                CardsByTriggerEventType = cardsByTriggerEventType
            };
        }

        /// <summary>
        /// Internal helper function that removes a sensor from a location.
        ///
        /// Should only be used as part of a larger operation as it does not
        /// add the sensor to any other collection, or destroy it.
        /// </summary>
        /// <param name="sensorId">Unique id of the sensor to remove.</param>
        /// <param name="column">Current location of the sensor.</param>
        /// <param name="side">Current side of the sensor.</param>
        private GameKernel RemoveSensorFromLocationUnsafe(long sensorId, Column column, Side side)
        {
            ValidateSensorExists(sensorId);
            ValidateSensorLocation(sensorId, column);
            ValidateSensorSide(sensorId, side);

            return (column, side) switch
            {
                (Column.Left, Side.Top)
                    => this with
                    {
                        TopLeftSensors = TopLeftSensors.Remove(sensorId)
                    },
                (Column.Middle, Side.Top)
                    => this with
                    {
                        TopMiddleSensors = TopMiddleSensors.Remove(sensorId)
                    },
                (Column.Right, Side.Top)
                    => this with
                    {
                        TopRightSensors = TopRightSensors.Remove(sensorId)
                    },
                (Column.Left, Side.Bottom)
                    => this with
                    {
                        BottomLeftSensors = BottomLeftSensors.Remove(sensorId)
                    },
                (Column.Middle, Side.Bottom)
                    => this with
                    {
                        BottomMiddleSensors = BottomMiddleSensors.Remove(sensorId)
                    },
                (Column.Right, Side.Bottom)
                    => this with
                    {
                        BottomRightSensors = BottomRightSensors.Remove(sensorId)
                    },
                (_, _) => throw new NotImplementedException()
            };
        }

        private ImmutableList<long> CardIdsForLocation(Column column, Side side)
        {
            return (column, side) switch
            {
                (Column.Left, Side.Top) => TopLeftCards,
                (Column.Middle, Side.Top) => TopMiddleCards,
                (Column.Right, Side.Top) => TopRightCards,
                (Column.Left, Side.Bottom) => BottomLeftCards,
                (Column.Middle, Side.Bottom) => BottomMiddleCards,
                (Column.Right, Side.Bottom) => BottomRightCards,
                (_, _) => throw new NotImplementedException()
            };
        }

        private bool NeedsTrigger(CardBase cardBase, CardState state)
        {
            var eventType = cardBase.Triggered?.EventType;
            if (eventType != null)
            {
                switch (state)
                {
                    case CardState.InHand:
                        return cardBase.Triggered!.WhenInHand;
                    case CardState.InLibrary:
                        return cardBase.Triggered.WhenInDeck;
                    case CardState.Discarded:
                    case CardState.Destroyed:
                        return cardBase.Triggered.WhenDiscardedOrDestroyed;
                    case CardState.InPlay:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        #endregion

        #region Validation Methods

        private void ValidateCardExists(long cardId)
        {
            if (!Cards.ContainsKey(cardId))
            {
                throw new InvalidOperationException($"Unknown card {cardId}.");
            }
        }

        private void ValidateState(long cardId, params CardState[] cardStates)
        {
            if (!cardStates.Contains(CardStates[cardId]))
            {
                var card = Cards[cardId];

                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is in state {CardStates[cardId]}, "
                        + $"not {string.Join(" or ", cardStates)}."
                );
            }
        }

        private void ValidateSide(long cardId, Side side)
        {
            if (CardSides[cardId] != side)
            {
                var card = Cards[cardId];

                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is not on side {side}."
                );
            }
        }

        private void ValidateLocation(long cardId, Column column)
        {
            if (CardLocations[cardId] != column)
            {
                var card = Cards[cardId];

                throw new InvalidOperationException(
                    $"Card {card.Name} ({card.Id}) is not in column {column}."
                );
            }
        }

        private void ValidateSensorExists(long sensorId)
        {
            if (!Sensors.ContainsKey(sensorId))
            {
                throw new InvalidOperationException($"Unknown sensor {sensorId}.");
            }
        }

        private void ValidateSensorSide(long sensorId, Side side)
        {
            if (SensorSides[sensorId] != side)
            {
                throw new InvalidOperationException($"Sensor {sensorId} is not on side {side}.");
            }
        }

        private void ValidateSensorLocation(long sensorId, Column column)
        {
            if (SensorLocations[sensorId] != column)
            {
                throw new InvalidOperationException(
                    $"Sensor {sensorId} is not in column {column}."
                );
            }
        }

        #endregion

        #region CardInLocationListReference

        public struct CardInLocationListReference : IReadOnlyList<ICard>
        {
            public CardInLocationListReference(GameKernel kernel, IReadOnlyList<long> inner)
            {
                this.Kernel = kernel;
                this.Inner = inner;
            }

            GameKernel Kernel { get; }

            IReadOnlyList<long> Inner { get; }

            public readonly ICard this[int index] => Kernel.GetCardUnsafe(Inner[index]) as ICard;

            public readonly int Count => Inner.Count;

            public readonly IEnumerator<ICard> GetEnumerator()
            {
                var kernel = Kernel;
                return Inner.Select(kernel.GetCardUnsafe).Cast<ICard>().GetEnumerator();
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion
    }
}
