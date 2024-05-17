namespace Snapdragon.Fluent.Transforms
{
    public record WithZeroCost() : ICardTransform
    {
        public ICardInstance Apply(ICardInstance card)
        {
            return card.WithModification(new Modification(-1 * card.Cost, null, card)); // TODO: Determine if we need to add a different source
        }
    }
}
