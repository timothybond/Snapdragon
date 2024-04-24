using Snapdragon.Fluent.EventFilters;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class EventCard
    {
        /// <summary>
        /// Filters events to those that involve cards in the same location as the context item.
        /// </summary>
        public static IEventFilter<CardEvent, IObjectWithPossibleColumn> Here => new HereFilter();

        /// <summary>
        /// Filters events to those that involve cards in the same location as the context <see cref="Sensor{Card}"/>.
        ///
        /// Technically this does the same thing as <see cref="Here"/> but it provides type info in
        /// a more succinct way in some contexts.
        /// </summary>
        public static IEventFilter<CardEvent, Sensor<ICard>> AtSensor => new HereFilter();

        /// <summary>
        /// Filters events to those on the same side as the contex item.
        /// </summary>
        public static IEventFilter<CardEvent, IObjectWithSide> SameSide => new ThisSideFilter();

        /// <summary>
        /// Filters events to those on the oppose side as the contex item.
        /// </summary>
        public static IEventFilter<CardEvent, IObjectWithSide> OtherSide => new OtherSideFilter();

        /// <summary>
        /// Filters events to those on different cards than the contex item.
        /// </summary>
        public static IEventFilter<CardEvent, ICardInstance> OtherCards => new OtherCards();

        public static ISingleItemSelector<Player, CardEvent, object> Player =>
            new EventCardSide<object>();

        /// <summary>
        /// Filters events to those that specifically involve the context card.
        /// </summary>
        public static IEventFilter<CardEvent, ICardInstance> Self => new Self();

        public static ISingleItemSelector<ICardInstance, CardEvent, object> Get => new EventCardSelector();
    }
}
