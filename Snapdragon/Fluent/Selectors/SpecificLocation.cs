namespace Snapdragon.Fluent.Selectors
{
    public record SpecificLocation(Column Column) : ISingleItemSelector<Location, object>
    {
        public Location? GetOrDefault(object context, Game game)
        {
            return game[Column];
        }
    }
}
