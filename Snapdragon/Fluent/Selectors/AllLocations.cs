namespace Snapdragon.Fluent.Selectors
{
    public record AllLocations : ISelector<Location, object>
    {
        public IEnumerable<Location> Get(object context, Game game)
        {
            return game.Locations;
        }

        public bool Selects(Location item, object context, Game game)
        {
            return true;
        }
    }
}
