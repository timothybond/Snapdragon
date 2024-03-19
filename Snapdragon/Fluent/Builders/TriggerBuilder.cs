namespace Snapdragon.Fluent.Builders
{
    public record TriggerBuilder<TEvent, TContext>
        : IBuilder<ITriggeredAbility<TContext>, TContext, IEffectBuilder<TEvent, TContext>>
        where TEvent : Event
        where TContext : class
    {
        public ITriggeredAbility<TContext> Build(IEffectBuilder<TEvent, TContext> outcome)
        {
            return new TriggeredAbility<TEvent, TContext>(outcome, null);
        }

        public TriggerBuilderWithFilter<TEvent, TContext> Where(
            IEventFilter<TEvent, TContext> eventFilter
        )
        {
            return new TriggerBuilderWithFilter<TEvent, TContext>(eventFilter);
        }
    }

    public record TriggerBuilderWithFilter<TEvent, TContext>(
        IEventFilter<TEvent, TContext> EventFilter
    ) : IBuilder<ITriggeredAbility<TContext>, TContext, IEffectBuilder<TEvent, TContext>>
        where TEvent : Event
        where TContext : class
    {
        public ITriggeredAbility<TContext> Build(IEffectBuilder<TEvent, TContext> outcome)
        {
            return new TriggeredAbility<TEvent, TContext>(outcome, EventFilter);
        }
    }
}
