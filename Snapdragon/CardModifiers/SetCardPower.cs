namespace Snapdragon.CardModifiers
{
    public record SetCardPower(int Amount) : ICardModifier
    {
        public Card Apply(Card card)
        {
            // TODO: Figure out how to apply effect limits (e.g. Luke Cage)
            return card with { Power = Amount };
        }
    }
}
