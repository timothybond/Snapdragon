namespace Snapdragon.Fluent.Selectors
{
    public record SpecificLocation(Column Column) : ISingleItemSelector<Location, object>
    {
        public Location? GetOrDefault(object context, Game game)
        {
            return game[Column];
        }

        public bool Selects(Location item, object context, Game game)
        {
            return item.Column == this.Column;
        }
    }
}
