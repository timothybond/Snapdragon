namespace Snapdragon.Effects
{
    public record AddPowerToCard(ICardInstance Card, int Amount, object Source) : ModifyCard(Card)
    {
        protected override CardBase ApplyToCard(CardBase card, Game game)
        {
            return card.WithModification(new Modification(null, Amount, Source));
        }
    }
}
