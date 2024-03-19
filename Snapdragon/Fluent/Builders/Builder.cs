namespace Snapdragon.Fluent.Builders
{
    public record Builder<TAbility, TContext, TOutcome>(
        IResultFactory<TAbility, TContext, TOutcome> Factory
    ) : IBuilder<TAbility, TContext, TOutcome>
    {
        public ConditionBuilder<TAbility, TContext, TOutcome> If
        {
            get { return new ConditionBuilder<TAbility, TContext, TOutcome>(Factory); }
        }

        public virtual TAbility Build(TOutcome outcome)
        {
            return Factory.Build(outcome);
        }
    }
}
