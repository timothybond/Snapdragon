namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Selects a random one of the items returned by some other selector.
    /// </summary>
    /// <param name="Selector">The base selector; one of its chosen items will be returned (if any).</param>
    public record RandomSingleItem<TSelected, TContext> : ISingleItemFilter<TSelected, TContext>
    {
        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return initial.OrderBy(i => Random.Next()).FirstOrDefault();
        }
    }
}
