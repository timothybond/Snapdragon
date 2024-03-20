namespace Snapdragon.Fluent.Selectors
{
    public record BothSides : ISideSelector<object>
    {
        public IEnumerable<Side> Get(object context, Game game)
        {
            yield return Side.Top;
            yield return Side.Bottom;
        }
    }
}
