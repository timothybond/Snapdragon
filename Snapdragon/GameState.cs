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
        bool GameOver = false)
    {
        public GameState WithEvent(Event e)
        {
            return this with { NewEvents = this.NewEvents.Add(e) };
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
