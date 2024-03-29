namespace Snapdragon.TargetFilters
{
    /// <summary>
    /// A filter for the only card at a given location (and side).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record OnlyCardHere<T> : ICardFilter<T>
        where T : IObjectWithColumn
    {
        public bool Applies(ICardInstance card, T source, Game game)
        {
            if (card.Column != source.Column)
            {
                return false;
            }

            if (game[source.Column][card.Side].Count > 1)
            {
                return false;
            }

            // Note: these are checks for mangled logic somewhere, because we haven't yet perfectly
            // guaranteed that cards are in the columns they say they're in.
            if (game[source.Column][card.Side].Count == 0)
            {
                throw new InvalidOperationException(
                    "Invalid state - checking for solo card, but it wasn't at its specified column."
                );
            }

            if (game[source.Column][card.Side][0].Id != card.Id)
            {
                throw new InvalidOperationException(
                    "Invalid state - checking for solo card, but some other card was the only one at its specified column."
                );
            }

            return true;
        }
    }
}
