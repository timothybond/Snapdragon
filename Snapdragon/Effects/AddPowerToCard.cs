namespace Snapdragon.Effects
{
    public record AddPowerToCard(ICardInstance Card, int Amount, object Source) : ModifyCard(Card)
    {
        protected override CardBase ApplyToCard(CardBase card, Game game)
        {
            return card with
            {
                Modifications = card.Modifications.Add(new Modification(null, Amount, Source))
            };
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
