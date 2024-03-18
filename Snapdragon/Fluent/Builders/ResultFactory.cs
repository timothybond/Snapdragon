namespace Snapdragon.Fluent.Builders
{
    public interface IResultFactory<TAbility, TContext, TOutcome>
    {
        public TAbility Build(ICondition<TContext> context, TOutcome outcome);
    }
}
