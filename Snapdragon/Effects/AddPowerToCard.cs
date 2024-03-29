namespace Snapdragon.Effects
{
    public record AddPowerToCard(ICardInstance Card, int Amount) : ModifyCard(Card)
    {
        protected override CardBase ApplyToCard(CardBase card, Game game)
        {
            return card with { Power = card.Power + Amount };
        }

        protected override bool IsBlocked(ICardInstance card, Game game)
        {
            if (Amount < 0)
            {
                if (card is ICard cardInLocation)
                {
                    var blockedEffects = game.GetBlockedEffects(cardInLocation);
                    if (blockedEffects.Contains(EffectType.ReducePower))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
