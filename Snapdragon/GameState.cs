﻿using System.Collections.Immutable;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using Snapdragon.Events;

namespace Snapdragon
{
    public record GameState(
        int Turn,
        Location Left,
        Location Middle,
        Location Right,
        Player Top,
        Player Bottom,
        Side FirstRevealed,
        ImmutableList<Event> PastEvents,
        ImmutableList<Event> NewEvents,
        bool GameOver = false
    )
    {
        /// <summary>
        /// Gets a modified state with the specified new <see cref="Event"/> added.
        /// </summary>
        public GameState WithEvent(Event e)
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
        /// Gets all <see cref="Card"/>s that have been played, whether or not they are revealed.
        /// </summary>
        public IReadOnlyList<Card> AllCards =>
            this.Left.AllCards.Concat(this.Middle.AllCards).Concat(this.Right.AllCards).ToList();

        /// <summary>
        /// Gets a modified state that includes the passed-in <see cref="Player"/> as appropriate.
        /// </summary>
        public GameState WithPlayer(Player player)
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
        public GameState WithLocation(Location location)
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

        public GameState WithRevealedLocation(Column column)
        {
            var location = this[column] with { Revealed = true };

            return this.WithLocation(location)
                .WithEvent(new LocationRevealedEvent(this.Turn, location));
        }

        /// <summary>
        /// Gets a modified state that applies some change to a <see cref="Card"/> (in place).
        ///
        /// Moves or side changes need to be handled elsewhere.
        /// </summary>
        /// <param name="currentCard">The existing card to be modified.</param>
        /// <param name="modifier">The modification to perform on the existing card.</param>
        /// <param name="postModifyTransform">
        /// Any change to the <see cref="GameState"/> to follow the modification
        /// (typically, this will be used to raise events, like <see cref="CardRevealedEvent"/>).
        /// </param>
        public GameState WithModifiedCard(
            Card currentCard,
            Func<Card, Card> modifier,
            Func<GameState, Card, GameState>? postModifyTransform = null
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

            var newState = this.WithLocation(location);

            if (postModifyTransform != null)
            {
                newState = postModifyTransform(newState, newCard);
            }

            return newState;
        }

        /// <summary>
        /// Get the <see cref="Side"/> of the <see cref="Player"/> who is currently winning,
        /// meaning they have control of more <see cref="Locations"/> or, in the event of a
        /// tie, they have more Power overall.
        /// </summary>
        /// <returns>The <see cref="Side"/> of the <see cref="Player"/> currently in the lead,
        /// or <c>null</c> if they are tied in both <see cref="Location"/>s and Power.</returns>
        public Side? GetLeader()
        {
            var topLocations = 0;
            var topPower = 0;
            var bottomLocations = 0;
            var bottomPower = 0;

            // TODO: Account for modifiers to Card Power
            foreach (var location in new[] { Left, Middle, Right })
            {
                var topPowerLocal = 0;
                var bottomPowerLocal = 0;

                foreach (var topCard in location.TopPlayerCards)
                {
                    topPowerLocal += topCard.Power;
                }

                foreach (var bottomCard in location.BottomPlayerCards)
                {
                    bottomPowerLocal += bottomCard.Power;
                }

                if (topPowerLocal > bottomPowerLocal)
                {
                    topLocations += 1;
                }
                else if (topPowerLocal < bottomPowerLocal)
                {
                    bottomLocations += 1;
                }

                topPower += topPowerLocal;
                bottomPower += bottomPowerLocal;
            }

            if (topLocations > bottomLocations)
            {
                return Side.Top;
            }
            else if (topLocations < bottomLocations)
            {
                return Side.Bottom;
            }
            else if (topPower > bottomPower)
            {
                return Side.Top;
            }
            else if (topPower < bottomPower)
            {
                return Side.Bottom;
            }

            return null;
        }
    }
}
