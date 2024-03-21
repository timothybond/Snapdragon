namespace Snapdragon.Fluent.EventFilters
{
    public record AndEventFilter<TEvent, TContext>(
        IEventFilter<TEvent, TContext> First,
        IEventFilter<TEvent, TContext> Second
    ) : IEventFilter<TEvent, TContext>
        where TEvent : Event
    {
        public bool Includes(TEvent e, TContext context, Game game)
        {
            return First.Includes(e, context, game) && Second.Includes(e, context, game);
        }
    }
}
