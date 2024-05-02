namespace Snapdragon.Fluent.Filters
{
    /// <summary>
    /// A selector for locations other than the one where the context object is.
    ///
    /// Note this is defined for items that may not have a location (in which case all locations are returned).
    /// </summary>
    public record OtherLocationsFilter : ISelector<Location, IObjectWithPossibleColumn>
    {
        public IEnumerable<Location> Get(IObjectWithPossibleColumn context, Game game)
        {
            return game.Locations.Where(l => l.Column != context.Column);
        }

        public bool Selects(Location item, IObjectWithPossibleColumn context, Game game)
        {
            return item.Column != context.Column;
        }
    }
}
