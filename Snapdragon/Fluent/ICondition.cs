namespace Snapdragon.Fluent
{
    public interface ICondition<in TContext> : ICondition<Event, TContext>
    {
        bool IsMet(TContext context, Game game);

        bool ICondition<Event, TContext>.IsMet(Event e, TContext context, Game game)
        {
            return this.IsMet(context, game);
        }
    }

    public interface ICondition<in TEvent, in TContext>
        where TEvent : Event
    {
        bool IsMet(TEvent e, TContext context, Game game);
    }
}
