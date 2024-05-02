namespace Snapdragon.Fluent.Ongoing
{
    public record OngoingAdjustLocationPower<TContext>(
        ISelector<Location, TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(OngoingAbilityType.AddPowerToLocation, Condition)
    { }
}
