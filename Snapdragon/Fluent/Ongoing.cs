namespace Snapdragon.Fluent
{
    public record Ongoing<TContext>(
        OngoingAbilityType Type,
        ICondition<TContext>? Condition = null
    ) { }
}
