namespace Snapdragon.TargetFilters
{
    public record SameLocation : ICardFilter<Card>, ILocationFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            return (card.Column == source.Column);
        }

        public bool Applies(Location location, Card source, Game game)
        {
            return (source.Column == location.Column);
        }
    }
}
