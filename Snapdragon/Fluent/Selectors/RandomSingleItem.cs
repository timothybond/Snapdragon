namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Selects a random one of the items returned by some other selector.
    /// </summary>
    /// <param name="Selector">The base selector; one of its chosen items will be returned (if any).</param>
    public record RandomSingleItem<TSelected, TContext> : ISingleItemFilter<TSelected, TContext>
    {
        public bool Applies(TSelected item, TContext context, Game game)
        {
            throw new NotImplementedException(
                $"The '{nameof(RandomSingleItem<TSelected, TContext>)} filter cannot be used "
                    + $"in a context in which we need to check a single item."
            );
        }

        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return initial.OrderBy(i => Random.Next()).FirstOrDefault();
        }
    }
}
