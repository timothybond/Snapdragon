namespace Snapdragon.OngoingAbilities
{
    public record OngoingAdjustPower<T>(ICardFilter<T> TargetFilter, IPowerCalculation<T> Amount) : IOngoingAbility<T>
    {
        public int? Apply(Card target, T source, Game game)
        {
            if (!TargetFilter.Applies(target, source, game))
            {
                return null;
            }

            return Amount.GetValue(game, source, target);
        }
    }
}
