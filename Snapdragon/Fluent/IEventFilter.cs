namespace Snapdragon.Fluent
{
    /// <summary>
    /// A filter for specific events.
    /// Can also be used as a condition that applies on an event, with the same basic logic.
    /// </summary>
    public interface IEventFilter<in TEvent, in TContext> : ICondition<TEvent, TContext>
        where TEvent : Event
    {
        bool Includes(TEvent e, TContext context, Game game);

        bool ICondition<TEvent, TContext>.IsMet(TEvent e, TContext context, Game game)
        {
            return this.Includes(e, context, game);
        }
    }
}
