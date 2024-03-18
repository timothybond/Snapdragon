namespace Snapdragon.Fluent.CardSelectors
{
    public record FilteredSelector<TContext>(
        ICardSelector<TContext> Selector,
        ICardFilter<TContext> Filter
    ) : ICardSelector<TContext>
    {
        public IEnumerable<ICard> Get(TContext context, Game game)
        {
            return Selector.Get(context, game).Where(c => Filter.Includes(c, context));
        }
    }
}
