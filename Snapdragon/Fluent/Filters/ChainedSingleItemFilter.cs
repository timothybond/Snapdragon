namespace Snapdragon.Fluent.Filters
{
    public record ChainedSingleItemFilter<TSelected, TContext>(
        IFilter<TSelected, TContext> First,
        ISingleItemFilter<TSelected, TContext> Second
    ) : ISingleItemFilter<TSelected, TContext>
    {
        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return Second.GetOrDefault(First.GetFrom(initial, context, game), context, game);
        }
    }
}
