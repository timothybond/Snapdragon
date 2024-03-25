namespace Snapdragon.Effects
{
    public record SwapCostAndPower(ICard Card) : ModifyCard(Card)
    {
        protected override ICard ApplyToCard(ICard card, Game game)
        {
            return card.ToCardInstance() with
            {
                Power = card.Cost,
                Cost = Math.Max(0, Math.Min(6, card.Power))
            };
        }

        protected override bool IsBlocked(ICard card, Game game)
        {
            return false;
        }
    }
}
