namespace Snapdragon.CardAbilities
{
    public record OngoingAdjustPower(ICardFilter TargetFilter, IPowerCalculation Amount)
        : ICardOngoingAbility
    {
        public int? Apply(Card target, Card source, GameState game)
        {
            if (!TargetFilter.Applies(target, source))
            {
                return null;
            }

            return Amount.GetValue(game, source, target);
        }
    }
}
