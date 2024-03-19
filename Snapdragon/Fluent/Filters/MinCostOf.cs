namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets all cards from the given set that are tied for minimum Cost.
    /// </summary>
    public record MinCostOf : ICardFilter<object>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, object context)
        {
            var cards = initial.ToList();
            if (cards.Count == 0)
            {
                return cards;
            }

            var minCost = cards.Min(c => c.Cost);
            return cards.Where(c => c.Cost == minCost);
        }
    }
}
