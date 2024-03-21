using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent
{
    public static class EventFilterExtensions
    {
        public static IEventFilter<TEvent, TContext> And<TEvent, TContext>(
            this IEventFilter<TEvent, TContext> first,
            IEventFilter<TEvent, TContext> second
        )
            where TEvent : Event
        {
            return new AndEventFilter<TEvent, TContext>(first, second);
        }
    }
}
