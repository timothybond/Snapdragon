namespace Snapdragon.Fluent.Builders
{
    public record AdjustPowerFactory<TContext>(ISelector<ICardInstance, TContext> Selector, int Amount)
        : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingAdjustPower<TContext>(Selector, Amount, condition);
        }
    }
}
