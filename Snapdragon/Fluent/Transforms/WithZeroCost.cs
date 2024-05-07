namespace Snapdragon.Fluent.Transforms
{
    public record WithZeroCost : ICardTransform
    {
        public CardBase Apply(CardBase card, object source)
        {
            return card.WithModification(new Modification(-1 * card.Cost, null, source));
        }
    }
}
