namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Selects a random subset of the items returned by the given selector.
    /// </summary>
    /// <param name="Selector">The base selector; some subset of its chosen items will be returned.</param>
    /// <param name="Number">How many items to return (at a maximum - there's no guarantee there will be any).</param>
    public record RandomCard<TContext>(ICardSelector<TContext> Selector, int Number = 1)
        : ICardSelector<TContext>
    {
        public IEnumerable<ICard> Get(TContext context, Game game)
        {
            return Selector.Get(context, game).OrderBy(i => Random.Next()).Take(Number);
        }
    }
}
