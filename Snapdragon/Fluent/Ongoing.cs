namespace Snapdragon.Fluent
{
    public record Ongoing<TContext>(ICondition<TContext>? Condition = null) { }

    public record OngoingAdjustPower<TContext>(
        ICardSelector<TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }
}
