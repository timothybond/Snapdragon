namespace Snapdragon.Fluent.Transforms
{
    public record WithZeroCost : ICardTransform
    {
        public CardBase Apply(CardBase card, object source)
        {
            return card with
            {
                Cost = 0,
                Modifications = card.Modifications.Add(
                    new Modification(-1 * card.Cost, null, source)
                )
            };
        }
    }
}
