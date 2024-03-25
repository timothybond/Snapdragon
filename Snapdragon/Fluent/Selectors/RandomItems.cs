namespace Snapdragon.Fluent.Selectors
{
    /// <summary>
    /// Selects a random subset of the items returned by another selector.
    /// </summary>
    /// <param name="Number">How many items to return (at a maximum - there's no guarantee there will be any).</param>
    public record RandomItems<TSelected, TContext>(int Number = 1) : IFilter<TSelected, TContext>
    {
        public IEnumerable<TSelected> GetFrom(IEnumerable<TSelected> initial, TContext context, Game game)
        {
            return initial.OrderBy(i => Random.Next()).Take(Number);
        }
    }
}
