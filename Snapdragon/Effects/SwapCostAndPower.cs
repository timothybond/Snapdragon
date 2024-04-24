namespace Snapdragon.Effects
{
    public record SwapCostAndPower(ICardInstance Card, object Source) : ModifyCard(Card)
    {
        protected override CardBase ApplyToCard(CardBase card, Game game)
        {
            var newCost = Math.Max(0, Math.Min(6, card.Power));
            var newPower = card.Cost;

            var costMod = newCost - card.Cost;
            var powerMod = newPower - card.Power;

            return card with
            {
                Modifications = card.Modifications.Add(new Modification(costMod, powerMod, Source))
            };
        }

        protected override bool IsBlocked(ICardInstance card, Game game)
        {
            return false;
        }
    }
}
