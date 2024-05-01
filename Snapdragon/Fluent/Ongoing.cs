namespace Snapdragon.Fluent
{
    public record Ongoing<TContext>(ICondition<TContext>? Condition = null) { }
}
