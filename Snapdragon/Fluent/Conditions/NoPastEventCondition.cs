namespace Snapdragon.Fluent.Conditions
{
    public record NoPastEventCondition<TEvent, TContext>(
        IEventFilter<TEvent, TContext>? Filter = null
    ) : ICondition<TContext>
        where TEvent : Event
    {
        public bool IsMet(TContext context, Game game)
        {
            var pastEvents = game.PastEvents.OfType<TEvent>();

            if (Filter != null)
            {
                pastEvents = pastEvents.Where(e => Filter.Includes(e, context, game));
            }

            return !pastEvents.Any();
        }
    }
}
