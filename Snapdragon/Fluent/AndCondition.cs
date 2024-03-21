namespace Snapdragon.Fluent
{
    public record AndCondition<TContext>(ICondition<TContext> First, ICondition<TContext> Second)
        : ICondition<TContext>
    {
        public bool IsMet(TContext context, Game game)
        {
            return First.IsMet(context, game) && Second.IsMet(context, game);
        }
    }

    public record AndCondition<TEvent, TContext>(
        ICondition<TEvent, TContext> First,
        ICondition<TEvent, TContext> Second
    ) : ICondition<TEvent, TContext>
        where TEvent : Event
    {
        public bool IsMet(TEvent e, TContext context, Game game)
        {
            return First.IsMet(e, context, game) && Second.IsMet(e, context, game);
        }
    }
}
