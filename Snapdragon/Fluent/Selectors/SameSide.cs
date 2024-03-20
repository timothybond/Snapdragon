namespace Snapdragon.Fluent.Selectors
{
    public record SameSide : ISideSelector<ICard>
    {
        public IEnumerable<Side> Get(ICard context, Game game)
        {
            yield return context.Side;
        }
    }
}
