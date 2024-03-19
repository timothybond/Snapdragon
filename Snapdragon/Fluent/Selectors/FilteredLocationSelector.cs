namespace Snapdragon.Fluent.Selectors
{
    public record FilteredLocationSelector<TContext>(
        ILocationSelector<TContext> Selector,
        ILocationFilter<TContext> Filter
    ) : ILocationSelector<TContext>
    {
        public IEnumerable<Location> Get(TContext context, Game game)
        {
            return Filter.GetFrom(Selector.Get(context, game), context);
        }
    }
}
