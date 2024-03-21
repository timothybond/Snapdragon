namespace Snapdragon.Fluent.Builders
{
    public interface IResultFactory<TAbility, TContext, TOutcome>
    {
        public TAbility Build(TOutcome outcome, ICondition<TContext>? condition = null);
    }

    public interface IResultFactory<TAbility, TEvent, TContext, TOutcome>
        where TEvent : Event
    {
        public TAbility Build(
            TOutcome outcome,
            IEventFilter<TEvent, TContext>? eventFilter = null,
            ICondition<TEvent, TContext>? condition = null
        );
    }
}
