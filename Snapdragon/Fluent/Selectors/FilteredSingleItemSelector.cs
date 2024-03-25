namespace Snapdragon.Fluent.Selectors
{
    public record FilteredSingleItemSelector<TResult, TContext>(
        ISelector<TResult, TContext> Selector,
        ISingleItemFilter<TResult, TContext> Filter
    ) : ISingleItemSelector<TResult, TContext>
    {
        public TResult? GetOrDefault(TContext context, Game game)
        {
            return Filter.GetOrDefault(Selector.Get(context, game), context, game);
        }
    }

    public record FilteredSingleItemSelector<TResult, TEvent, TContext>(
        ISelector<TResult, TEvent, TContext> Selector,
        ISingleItemFilter<TResult, TEvent, TContext> Filter
    ) : ISingleItemSelector<TResult, TEvent, TContext>
        where TEvent : Event
    {
        public TResult? GetOrDefault(TEvent e, TContext context, Game game)
        {
            return Filter.GetOrDefault(Selector.Get(e, context, game), e, context, game);
        }
    }
}
