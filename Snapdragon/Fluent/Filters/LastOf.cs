﻿namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets the last card in a given selection (if any).
    ///
    /// (As far as I know this is only used to get the rightmost card in a player's hand, for Blade.)
    /// </summary>
    public record LastOf<TSelected> : ISingleItemFilter<TSelected, object>
    {
        public TSelected? GetOrDefault(IEnumerable<TSelected> initial, object context, Game game)
        {
            var items = initial.ToList();

            if (items.Count > 0)
            {
                return items[items.Count - 1];
            }

            return default;
        }
    }
}
