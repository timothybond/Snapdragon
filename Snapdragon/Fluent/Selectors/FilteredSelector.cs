namespace Snapdragon.Fluent.Selectors
{
    public record FilteredSelector<TResult, TContext>(
        ISelector<TResult, TContext> Selector,
        IFilter<TResult, TContext> Filter
    ) : ISelector<TResult, TContext>
    {
        public IEnumerable<TResult> Get(TContext context, Game game)
        {
            return Filter.GetFrom(Selector.Get(context, game), context, game);
        }

        public bool Selects(TResult item, TContext context, Game game)
        {
            return Selector.Selects(item, context, game) && Filter.Applies(item, context, game);
        }
    }

    public record FilteredSelector<TResult, TEvent, TContext>(
        ISelector<TResult, TEvent, TContext> Selector,
        IFilter<TResult, TEvent, TContext> Filter
    ) : ISelector<TResult, TEvent, TContext>
        where TEvent : Event
    {
        public IEnumerable<TResult> Get(TEvent e, TContext context, Game game)
        {
            return Filter.GetFrom(Selector.Get(e, context, game), e, context, game);
        }
    }
}
