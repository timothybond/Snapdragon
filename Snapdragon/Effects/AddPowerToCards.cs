namespace Snapdragon.Effects
{
    /// <summary>
    /// An <see cref="IEffect"/> that adds a calculated amount of power to
    /// the <see cref="CardInstance"/>s specified by the given <see cref="ICardFilter"/>.
    ///
    /// Note that this is permanent power, not as an ongoing effect.
    /// </summary>
    public record AddPowerToCards(ICardFilter Targets, ICalculation Amount) : IEffect
    {
        public Game Apply(Game game)
        {
            var power = Amount.GetValue(game);
            var cards = game
                .AllCards.Where(c => Targets.Applies(c, game))
                .Select(c =>
                {
                    var blockedEffects = game.GetBlockedEffects(c);

                    if (blockedEffects.Contains(EffectType.AdjustPower))
                    {
                        return c;
                    }

                    if (blockedEffects.Contains(EffectType.ReducePower) && power < 0)
                    {
                        return c;
                    }

                    return c with
                    {
                        Power = c.Power + power
                    };
                });

            return game.WithUpdatedCards(cards);
        }
    }
}
