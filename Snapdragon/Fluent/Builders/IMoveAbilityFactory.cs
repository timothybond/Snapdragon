namespace Snapdragon.Fluent.Builders
{
    public interface IMoveAbilityFactory<TContext>
    {
        IMoveAbility<TContext> Build(ICondition<TContext>? condition = null);
    }
}
