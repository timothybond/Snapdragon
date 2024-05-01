namespace Snapdragon.Fluent.Ongoing
{
    public record OngoingAdjustPower<TContext>(
        ISelector<ICardInstance, TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition)
    { }
}
