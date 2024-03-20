namespace Snapdragon.Fluent.Builders
{
    public record AdjustLocationPowerFactory<TContext>(
        ILocationSelector<TContext> Selector,
        int Amount
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingAdjustLocationPower<TContext>(Selector, Amount, condition);
        }
    }
}
