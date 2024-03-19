namespace Snapdragon.Fluent.Builders
{
    public interface IResultFactory<TAbility, TContext, TOutcome>
    {
        public TAbility Build(TOutcome outcome, ICondition<TContext>? condition = null);
    }
}
