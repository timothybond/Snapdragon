namespace Snapdragon.LocationFilters
{
    public record OtherLocations : ILocationFilter<ICard>, ILocationFilter<Location>
    {
        public bool Applies(Location location, ICard source, Game game)
        {
            return location.Column != source.Column;
        }

        public bool Applies(Location location, Location source, Game game)
        {
            return location.Column != source.Column;
        }
    }
}
