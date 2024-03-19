namespace Snapdragon.Fluent.Builders
{
    public interface IOngoingAbilityFactory<TContext>
    {
        Ongoing<TContext> Build(ICondition<TContext>? condition = null);
    }
}
