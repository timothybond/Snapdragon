using System.Collections.Immutable;
using System.Diagnostics;

namespace Snapdragon
{
    /// <summary>
    /// An object that stores some core details of the game state.
    ///
    /// Specifically, it tracks the location and status of all cards.
    ///
    /// The base constructor should not be used directly.
    /// </summary>
    public record GameKernel(
        ImmutableDictionary<long, ICard> Cards,
        ImmutableDictionary<long, Side> CardSides,
        ImmutableDictionary<long, CardState> CardStates,
        ImmutableDictionary<long, Column?> CardLocations,
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
        ImmutableList<long> BottomDestroyed
    )
    {
        public static GameKernel FromPlayers(Player top, Player bottom)
        {
            var cards = top
                .Library.Cards.Concat(bottom.Library.Cards)
                .Cast<ICard>()
                .ToImmutableDictionary(c => c.Id);
            var cardSides = top
                .Library.Cards.Select(c => (c.Id, Side.Top))
                .Concat(bottom.Library.Cards.Select(c => (c.Id, Side.Bottom)))
                .ToImmutableDictionary(kp => kp.Id, kp => kp.Item2);

            var cardStates = cards.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => CardState.InLibrary
            );
            var cardLocations = cards.ToImmutableDictionary(kvp => kvp.Key, kvp => (Column?)null);

            return new GameKernel(
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
                top.Library.Cards.Select(c => c.Id).ToImmutableList(),
                [],
                [],
                [],
                bottom.Library.Cards.Select(c => c.Id).ToImmutableList(),
                [],
                []
            );
        }

        public ImmutableList<long> this[Column column, Side side]
        {
            get
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

                    return this.RemoveCardFromLibraryUnsafe(cardId, side)
                        .WithCardInState(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);

                case Side.Bottom:
                    if (BottomLibrary.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "Cannot draw; bottom player library is empty."
                        );
                    }

                    cardId = BottomLibrary[0];

                    return this.RemoveCardFromLibraryUnsafe(cardId, side)
                        .WithCardInState(cardId, CardState.InHand)
                        .AddCardToHandUnsafe(cardId, side);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates the stored record for a card.
        /// </summary>
        /// <param name="card">The new card state to store.</param>
        public GameKernel WithUpdatedCard(ICard card)
        {
            return this with { Cards = Cards.SetItem(card.Id, card) };
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

            var newCollection = this[column, side];

            if (newCollection.Count + 1 > Max.CardsPerLocation)
            {
                // Technically redundant
                throw new InvalidOperationException(
                    $"Attempted to place {newCollection.Count + 1} items at {column}, {side} but max is {Max.CardsPerLocation}."
                );
            }

            newCollection = newCollection.Add(cardId);
            return this.RemoveCardFromHandUnsafe(cardId, side)
                .WithCardInState(cardId, CardState.PlayedButNotRevealed)
                .WithUpdatedCard( // TODO: Cleanup
                    card.ToCardInstance() with
                    {
                        State = CardState.PlayedButNotRevealed,
                        Column = column
                    }
                )
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
            ValidateState(cardId, CardState.InPlay);
            ValidateLocation(cardId, from);

            return this.RemoveCardFromLocationUnsafe(cardId, from, side)
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

            return this with
            {
                CardStates = CardStates.SetItem(cardId, CardState.InPlay)
            };
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
                    return this.RemoveCardFromHandUnsafe(cardId, side)
                        .WithCardInState(cardId, CardState.Discarded) with
                    {
                        TopDiscards = TopDiscards.Add(cardId)
                    };
                case Side.Bottom:
                    return this.RemoveCardFromHandUnsafe(cardId, side)
                        .WithCardInState(cardId, CardState.Discarded) with
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

            var kernel = this.RemoveCardFromLocationUnsafe(cardId, column, side)
                .WithCardInState(cardId, CardState.Destroyed);

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

            return this.RemoveCardFromGameUnsafe(cardId);
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

            return this.RemoveCardFromGameUnsafe(cardId);
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

            return this.RemoveCardFromDiscardsUnsafe(cardId, side)
                .WithCardInState(cardId, CardState.InPlay)
                .AddCardToLocationUnsafe(cardId, column, side);
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

            return this.RemoveCardFromDestroyedUnsafe(cardId, side)
                .WithCardInState(cardId, CardState.InPlay)
                .AddCardToLocationUnsafe(cardId, column, side);
        }

        #endregion

        #region Internal Logic

        /// <summary>
        /// Internal helper function that removes a card from the game.
        ///
        /// This is used for cases where items are destroyed from the hand or deck,
        /// in which case I believe they are unrecoverable.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        private GameKernel RemoveCardFromGameUnsafe(long cardId)
        {
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
                BottomDestroyed = BottomDestroyed.Remove(cardId)
            };
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
            ValidateState(cardId, CardState.InLibrary);

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
            ValidateLocation(cardId, column);
            ValidateSide(cardId, side);

            var existingCards = this[column, side];

            return this.WithUpdatedLocationCollection(column, side, existingCards.Add(cardId));
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
            ;
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

            return (column, side) switch
            {
                (Column.Left, Side.Top) => this with { TopLeftCards = newLocationItems },
                (Column.Middle, Side.Top) => this with { TopMiddleCards = newLocationItems },
                (Column.Right, Side.Top) => this with { TopRightCards = newLocationItems },
                (Column.Left, Side.Bottom) => this with { BottomLeftCards = newLocationItems },
                (Column.Middle, Side.Bottom) => this with { BottomMiddleCards = newLocationItems },
                (Column.Right, Side.Bottom) => this with { BottomRightCards = newLocationItems },
                (_, _) => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Internal helper function that udates the state record of a card.
        /// </summary>
        /// <param name="cardId">Unique identifier of the card.</param>
        /// <param name="state">New state to store for the card.</param>
        /// <returns></returns>
        private GameKernel WithCardInState(long cardId, CardState state)
        {
            return this with { CardStates = CardStates.SetItem(cardId, state) };
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

        #endregion
    }
}
