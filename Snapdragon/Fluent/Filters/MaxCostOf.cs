﻿namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets all cards from the given set that are tied for maximum Cost.
    /// </summary>
    public record MaxCostOf : ICardFilter<object>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, object context)
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
