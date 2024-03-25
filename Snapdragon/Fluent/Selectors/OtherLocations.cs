namespace Snapdragon.Fluent.Selectors
{
    public record OtherLocations<TContext> : ISelector<Location, TContext>
        where TContext : IObjectWithColumn
    {
        public IEnumerable<Location> Get(TContext context, Game game)
        {
            return game.Locations.Where(loc => loc.Column != context.Column);
        }
    }
}
