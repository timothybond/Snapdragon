namespace Snapdragon.CardModifiers
{
    public record SwapCostAndPower : ICardModifier
    {
        public CardInstance Apply(CardInstance card)
        {
            // TODO: Figure out if there's anything that stacks weirdly here, like Okoye
            return card with { Power = card.Cost, Cost = Math.Max(0, card.Power) };
        }
    }
}
