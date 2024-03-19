namespace Snapdragon.Fluent.Selectors
{
    public record FilteredCardSelector<TContext>(
        ICardSelector<TContext> Selector,
        ICardFilter<TContext> Filter
    ) : ICardSelector<TContext>
    {
        public IEnumerable<ICard> Get(TContext context, Game game)
        {
            return Filter.GetFrom(Selector.Get(context, game), context);
        }
    }
}
