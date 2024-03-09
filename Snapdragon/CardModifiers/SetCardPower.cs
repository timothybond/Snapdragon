namespace Snapdragon.CardModifiers
{
    public record SetCardPower(int Amount) : ICardModifier
    {
        public CardInstance Apply(CardInstance card)
        {
            // TODO: Figure out how to apply effect limits (e.g. Luke Cage)
            return card with { Power = Amount };
        }
    }
}
