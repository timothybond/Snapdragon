namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets the first card in a given selection (if any).
    ///
    /// (This is generally used to get the top card in a deck.)
    /// </summary>
    public record FirstOf<TSelected> : ISingleItemFilter<TSelected, object>
    {
        public bool Applies(TSelected item, object context, Game game)
        {
            throw new NotImplementedException(
                $"The '{nameof(FirstOf<TSelected>)} filter cannot be used " +
                $"in a context in which we need to check a single item."
            );
        }

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
