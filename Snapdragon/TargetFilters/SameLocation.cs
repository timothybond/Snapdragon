namespace Snapdragon.TargetFilters
{
    public record SameLocation<T> : ICardFilter<T>, ILocationFilter<T>
        where T : ICard
    {
        public bool Applies(ICard card, T source, Game game)
        {
            return (card.Column == source.Column);
        }

        public bool Applies(Location location, T source, Game game)
        {
            return (source.Column == location.Column);
        }
    }
}
