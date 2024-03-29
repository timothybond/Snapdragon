namespace Snapdragon.Fluent.Transforms
{
    public record WithZeroCost : ICardTransform
    {
        public CardBase Apply(CardBase card)
        {
            return card with { Cost = 0 };
        }
    }
}
