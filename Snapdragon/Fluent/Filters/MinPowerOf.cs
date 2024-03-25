namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A filter that gets all cards from the given set that are tied for minimum Power.
    /// </summary>
    public record MinPowerOf : IFilter<ICard, object>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, object context, Game game)
        {
            var cards = initial.ToList();
            if (cards.Count == 0)
            {
                return cards;
            }

            // TODO: See if we need to get adjusted power instead(?)
            var minPower = cards.Min(c => c.Power);
            return cards.Where(c => c.Power == minPower);
        }
    }
}
