namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets all cards from the given set that are tied for minimum Cost.
    /// </summary>
    public record MinCostOf : IFilter<ICardInstance, object>
    {
        public IEnumerable<ICardInstance> GetFrom(IEnumerable<ICardInstance> initial, object context, Game game)
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
