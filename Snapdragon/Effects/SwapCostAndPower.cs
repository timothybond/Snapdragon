namespace Snapdragon.Effects
{
    public record SwapCostAndPower(ICardInstance Card) : ModifyCard(Card)
    {
        protected override CardBase ApplyToCard(CardBase card, Game game)
        {
            return card with { Power = card.Cost, Cost = Math.Max(0, Math.Min(6, card.Power)) };
        }

        protected override bool IsBlocked(ICardInstance card, Game game)
        {
            return false;
        }
    }
}
