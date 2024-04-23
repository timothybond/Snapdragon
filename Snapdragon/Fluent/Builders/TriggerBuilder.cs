namespace Snapdragon.Fluent.Builders
{
    public record TriggerBuilder<TEvent, TContext>
        : Builder<ITriggeredAbility<TContext>, TEvent, TContext, IEffectBuilder<TEvent, TContext>>
        where TEvent : Event
        where TContext : class
    {
        public TriggerBuilder()
            : base(new TriggerFactory()) { }

        private class TriggerFactory
            : IResultFactory<
                ITriggeredAbility<TContext>,
                TEvent,
                TContext,
                IEffectBuilder<TEvent, TContext>
            >
        {
            public ITriggeredAbility<TContext> Build(
                IEffectBuilder<TEvent, TContext> outcome,
                IEventFilter<TEvent, TContext>? eventFilter = null,
                ICondition<TEvent, TContext>? condition = null
            )
            {
                return new TriggeredAbility<TEvent, TContext>(outcome, eventFilter, condition);
            }
        }
    }

    //public record TriggerBuilderWithFilter<TEvent, TContext>(
    //    IEventFilter<TEvent, TContext> EventFilter
    //) : IBuilder<ITriggeredAbility<TContext>, TContext, IEffectBuilder<TEvent, TContext>>
    //    where TEvent : Event
    //    where TContext : class
    //{
    //    public TriggerBuilderConditionBuilder<TEvent, TContext> If
    //    {
    //        get { return new TriggerBuilderConditionBuilder<TEvent, TContext>(EventFilter); }
    //    }

    //    public ITriggeredAbility<TContext> Build(IEffectBuilder<TEvent, TContext> outcome)
    //    {
    //        return new TriggeredAbility<TEvent, TContext>(outcome, EventFilter);
    //    }
    //}

    //public record TriggerBuilderConditionBuilder<TEvent, TContext>(
    //    IEventFilter<TEvent, TContext>? EventFilter
    //) : IConditionBuilder<ITriggeredAbility<TContext>, TContext, IEffectBuilder<TEvent, TContext>>
    //    where TEvent : Event
    //    where TContext : class
    //{
    //    public IBuilderWithCondition<
    //        ITriggeredAbility<TContext>,
    //        TContext,
    //        IEffectBuilder<TEvent, TContext>
    //    > WithCondition(ICondition<TContext> condition)
    //    {
    //        return new TriggerWithConditionBuilder<TEvent, TContext>(condition, EventFilter);
    //    }
    //}

    //public record TriggerWithConditionBuilder<TEvent, TContext>(
    //    ICondition<TContext> Condition,
    //    IEventFilter<TEvent, TContext>? EventFilter
    //)
    //    : IBuilderWithCondition<
    //        ITriggeredAbility<TContext>,
    //        TContext,
    //        IEffectBuilder<TEvent, TContext>
    //    >
    //    where TEvent : Event
    //    where TContext : class
    //{
    //    public IConditionBuilder<
    //        ITriggeredAbility<TContext>,
    //        TContext,
    //        IEffectBuilder<TEvent, TContext>
    //    > And
    //    {
    //        get
    //        {
    //            return new TriggerChainedConditionBuilder<TEvent, TContext>(Condition, EventFilter);
    //        }
    //    }

    //    public ITriggeredAbility<TContext> Build(IEffectBuilder<TEvent, TContext> outcome)
    //    {
    //        return new TriggeredAbility<TEvent, TContext>(outcome, EventFilter, Condition);
    //    }
    //}

    //public record TriggerChainedConditionBuilder<TEvent, TContext>(
    //    ICondition<TContext> Condition,
    //    IEventFilter<TEvent, TContext>? EventFilter
    //) : IConditionBuilder<ITriggeredAbility<TContext>, TContext, IEffectBuilder<TEvent, TContext>>
    //    where TEvent : Event
    //    where TContext : class
    //{
    //    public IBuilderWithCondition<
    //        ITriggeredAbility<TContext>,
    //        TContext,
    //        IEffectBuilder<TEvent, TContext>
    //    > WithCondition(ICondition<TContext> newCondition)
    //    {
    //        return new TriggerWithConditionBuilder<TEvent, TContext>(
    //            new AndCondition<TContext>(Condition, newCondition),
    //            EventFilter
    //        );
    //    }
    //}
}
