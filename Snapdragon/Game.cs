using Snapdragon.Events;
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
        bool GameOver = false)
    {
        /// <summary>
        /// Gets a modified state with the specified new <see cref="Event"/> added.
        /// </summary>
        public Game WithEvent(Event e)
        {
            return this with { NewEvents = this.NewEvents.Add(e) };
        }

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
        public IEnumerable<Card> AllCards => this
                .Left.AllCards
            .Concat(this.Middle.AllCards)
            .Concat(this.Right.AllCards)
            .Where(c => c.State == CardState.InPlay);

        /// <summary>
        /// Gets all <see cref="Card"/>s that have been played, whether or not they are revealed.
        /// </summary>
        public IEnumerable<Card> AllCardsIncludingUnrevealed => this.Left.AllCards
            .Concat(this.Middle.AllCards)
            .Concat(this.Right.AllCards);

        public IEnumerable<TemporaryEffect<Card>> AllCardTemporaryEffects => this
                .Left.TemporaryCardEffects
            .Concat(this.Middle.TemporaryCardEffects)
            .Concat(this.Right.TemporaryCardEffects);

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

            return this.WithLocation(location).WithEvent(new LocationRevealedEvent(this.Turn, location));
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
                card.Column ?? throw new InvalidOperationException("Tried to modify a card that isn't in play.");

            var location = this[column];
            var newCards = location[card.Side]
                .Select(c => c.Id == card.Id ? card : c)
                .ToImmutableList();

            location = location with
            {
                TopPlayerCards = card.Side == Side.Top ? newCards : location.TopPlayerCards,
                BottomPlayerCards = card.Side == Side.Bottom ? newCards : location.BottomPlayerCards,
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
            Func<Game, Card, Game>? postModifyTransform = null)
        {
            if (currentCard.Column == null)
            {
            }

            Column column =
                currentCard.Column ?? throw new InvalidOperationException("Tried to modify a card that isn't in play.");

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

        /// <summary>
        /// Gets the modified state after ending the current turn and processing any raised events.
        /// </summary>
        /// <returns></returns>
        public Game EndTurn()
        {
            return this.WithEvent(new TurnEndedEvent(this.Turn));
        }

        public Game ProcessNextEvent()
        {
            if (NewEvents.Count == 0)
            {
                return this;
            }

            var nextEvent = NewEvents[0];
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

        public Game RecalculateOngoingEffects()
        {
            var ongoingCardAbilities = this.GetCardOngoingAbilities().ToList();

            var recalculatedCards = this.AllCards
                .Select(
                    c => c with
                    {
                        PowerAdjustment = this.GetPowerAdjustment(c, ongoingCardAbilities, this)
                    });

            return this.WithCards(recalculatedCards);
        }

        private int? GetPowerAdjustment(
            Card card,
            IReadOnlyList<(IOngoingAbility<Card> Ability, Card Source)> ongoingCardAbilities,
            Game game)
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
    }
}
