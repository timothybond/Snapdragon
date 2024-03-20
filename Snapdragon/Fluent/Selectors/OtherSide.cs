namespace Snapdragon.Fluent.Selectors
{
    public record OtherSide : ISideSelector<ICard>
    {
        public IEnumerable<Side> Get(ICard context, Game game)
        {
            yield return context.Side.Other();
        }
    }
}
