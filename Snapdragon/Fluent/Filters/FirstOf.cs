namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets the first card in a given selection (if any).
    ///
    /// (This is generally used to get the top card in a deck.)
    /// </summary>
    public record FirstOf<TSelected> : ISingleItemFilter<TSelected, object>
    {
        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, object context, Game game)
        {
            var items = initial.ToList();

            if (items.Count > 0)
            {
                return items[0];
            }

            return default;
        }
    }
}
