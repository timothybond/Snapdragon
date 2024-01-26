using System.Collections.Immutable;

namespace Snapdragon
{
    public record GameState(
        int Turn,
        Location Left,
        Location Middle,
        Location Right,
        Player Top,
        Player Bottom,
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

                if (topPower > bottomPower)
                {
                    topLocations += 1;
                }
                else if (topPower < bottomPower)
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
