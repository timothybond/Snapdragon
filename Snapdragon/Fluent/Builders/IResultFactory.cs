namespace Snapdragon.Fluent.Builders
{
    public interface IResultFactory<TAbility, TContext, TOutcome>
    {
        public TAbility Build(TOutcome outcome, ICondition<TContext>? condition = null);
    }

    public interface IResultFactory<TAbility, TEvent, TContext, TOutcome>
    {
        public TAbility Build(
            TOutcome outcome,
            IEventFilter<TEvent, TContext>? eventFilter = null,
            ICondition<TContext>? condition = null
        );
    }
}
