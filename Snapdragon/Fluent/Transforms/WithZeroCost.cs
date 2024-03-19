namespace Snapdragon.Fluent.Transforms
{
    public record WithZeroCost : ICardTransform
    {
        public ICard Apply(ICard card)
        {
            return card.ToCardInstance() with { Cost = 0 };
        }
    }
}
