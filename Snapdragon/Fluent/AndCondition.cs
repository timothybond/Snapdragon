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
}
