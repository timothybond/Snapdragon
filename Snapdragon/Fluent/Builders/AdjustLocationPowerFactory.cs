namespace Snapdragon.Fluent.Builders
{
    public record AdjustLocationPowerFactory<TContext>(
        ISelector<Location, TContext> Selector,
        int Amount
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingAdjustLocationPower<TContext>(Selector, Amount, condition);
        }
    }
}
