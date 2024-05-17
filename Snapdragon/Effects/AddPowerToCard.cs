namespace Snapdragon.Effects
{
    public record AddPowerToCard(ICardInstance Card, int Amount, object Source) : ModifyCard(Card)
    {
        protected override ICardInstance ApplyToCard(ICardInstance card, Game game)
        {
            return card.WithModification(new Modification(null, Amount, Source));
        }
    }
}
