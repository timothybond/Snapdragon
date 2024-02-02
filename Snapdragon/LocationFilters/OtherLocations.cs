namespace Snapdragon.LocationFilters
{
    public record OtherLocations : ILocationFilter<Card>, ILocationFilter<Location>
    {
        public bool Applies(Location location, Card source, Game game)
        {
            return location.Column != source.Column;
        }

        public bool Applies(Location location, Location source, Game game)
        {
            return location.Column != source.Column;
        }
    }
}
