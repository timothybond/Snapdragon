namespace Snapdragon.CardAbilities
{
    public record OngoingAdjustPower<T>(ICardFilter<T> TargetFilter, IPowerCalculation<T> Amount)
        : IOngoingAbility<T>
    {
        public int? Apply(Card target, T source, GameState game)
        {
            if (!TargetFilter.Applies(target, source))
            {
                return null;
            }

            return Amount.GetValue(game, source, target);
        }
    }
}
