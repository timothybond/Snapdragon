namespace Snapdragon.LocationFilters
{
    public record SpecificLocation(Column Column) : ILocationFilter<object>
    {
        public bool Applies(Location location, object source, Game game)
        {
            return location.Column == Column;
        }
    }
}
