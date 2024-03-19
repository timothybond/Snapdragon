using Snapdragon.Fluent.EventFilters;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class EventCard
    {
        /// <summary>
        /// Filters events to those that involve cards in the same location as the context card.
        /// </summary>
        public static IEventFilter<ICardEvent, IObjectWithPossibleColumn> Here => new HereFilter();

        /// <summary>
        /// Filters events to those on the same side as the contex card.
        /// </summary>
        public static IEventFilter<ICardEvent, ICard> SameSide => new ThisSideFilter();

        /// <summary>
        /// Filters events to those that specifically involve the context card.
        /// </summary>
        public static IEventFilter<ICardEvent, ICard> Self => new Self();
    }
}
