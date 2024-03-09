﻿namespace Snapdragon.CardModifiers
{
    public record ModifyCardPower(int Amount) : ICardModifier
    {
        public CardInstance Apply(CardInstance card)
        {
            // TODO: Figure out how to apply effect limits (e.g. Luke Cage)
            return card with { Power = card.Power + Amount };
        }
    }
}
