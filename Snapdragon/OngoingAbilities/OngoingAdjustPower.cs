namespace Snapdragon.OngoingAbilities
{
    public record OngoingAdjustPower<T>(ICardFilter<T> TargetFilter, IPowerCalculation<T> Amount)
        : IOngoingAbility<T>
    {
        public int? Apply(Card target, T source, Game game)
        {
            if (!TargetFilter.Applies(target, source, game))
            {
                return null;
            }

            var blockedEffects = game.GetBlockedEffects(target);

            if (blockedEffects.Contains(EffectType.AdjustPower))
            {
                return null;
            }

            var amount = Amount.GetValue(game, source, target);

            if (amount < 0 && blockedEffects.Contains(EffectType.ReducePower))
            {
                return null;
            }

            return amount;
        }
    }
}
