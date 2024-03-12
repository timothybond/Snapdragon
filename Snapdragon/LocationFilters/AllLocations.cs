namespace Snapdragon.LocationFilters
{
    public record AllLocations<T> : ILocationFilter<T>
    {
        public bool Applies(Location location, T source, Game game)
        {
            return true;
        }
    }
}
