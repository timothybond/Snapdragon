namespace Snapdragon.Fluent.Filters
{
    public record ChainedSingleItemFilter<TSelected, TContext>(
        IFilter<TSelected, TContext> First,
        ISingleItemFilter<TSelected, TContext> Second
    ) : ISingleItemFilter<TSelected, TContext>
    {
        public bool Applies(TSelected item, TContext context, Game game)
        {
            return First.Applies(item, context, game) && Second.Applies(item, context, game);
        }

        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return Second.GetOrDefault(First.GetFrom(initial, context, game), context, game);
        }
    }
}
