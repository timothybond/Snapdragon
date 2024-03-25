namespace Snapdragon.Fluent.Filters
{
    public record ChainedFilter<TSelected, TContext>(
        IFilter<TSelected, TContext> First,
        IFilter<TSelected, TContext> Second
    ) : IFilter<TSelected, TContext>
    {
        public IEnumerable<TSelected> GetFrom(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return Second.GetFrom(First.GetFrom(initial, context, game), context, game);
        }
    }
}
