namespace Snapdragon.LocationFilters
{
    public record AllLocations : ILocationFilter<object>
    {
        public bool Applies(Location location, object source, Game game)
        {
            return true;
        }
    }
}
