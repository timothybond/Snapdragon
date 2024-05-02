namespace Snapdragon.Fluent.Selectors
{
    public record OtherLocations<TContext> : ISelector<Location, TContext>
        where TContext : IObjectWithColumn
    {
        public IEnumerable<Location> Get(TContext context, Game game)
        {
            return game.Locations.Where(loc => loc.Column != context.Column);
        }

        public bool Selects(Location item, TContext context, Game game)
        {
            return item.Column != context.Column;
        }
    }
}
