namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets all cards from the given set that are tied for maximum Cost.
    /// </summary>
    public record MaxCostOf : IFilter<ICardInstance, object>
    {
        public bool Applies(ICardInstance item, object context, Game game)
        {
            throw new NotImplementedException(
                $"The '{nameof(MaxCostOf)} filter cannot be used " +
                $"in a context in which we need to check a single item."
            );
        }

        public IEnumerable<ICardInstance> GetFrom(IEnumerable<ICardInstance> initial, object context, Game game)
        {
            var cards = initial.ToList();
            if (cards.Count == 0)
            {
                return cards;
            }

            var maxCost = cards.Max(c => c.Cost);
            return cards.Where(c => c.Cost == maxCost);
        }
    }
}
