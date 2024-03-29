namespace Snapdragon.TargetFilters
{
    public record CardsHere
        : ICardFilter<IObjectWithPossibleColumn>,
            ILocationFilter<IObjectWithPossibleColumn>
    {
        public bool Applies(ICardInstance card, IObjectWithPossibleColumn source, Game game)
        {
            return (card.Column == source.Column);
        }

        public bool Applies(Location location, IObjectWithPossibleColumn source, Game game)
        {
            return (source.Column == location.Column);
        }
    }
}
