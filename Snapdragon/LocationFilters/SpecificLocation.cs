namespace Snapdragon.LocationFilters
{
    public record SpecificLocation<T>(Column Column) : ILocationFilter<T>
    {
        public bool Applies(Location location, T source, Game game)
        {
            return location.Column == Column;
        }
    }
}
