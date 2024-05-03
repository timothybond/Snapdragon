namespace Snapdragon.Fluent
{
    public record TriggeredAbility<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter,
        ICondition<TEvent, TContext>? Condition = null
    ) : BaseTriggeredAbility<TContext, TEvent>(), ISpecialCardTrigger
        where TEvent : Event
    {
        public override bool WhenDiscardedOrDestroyed => false;
        public override bool WhenInHand => false;
        public override bool WhenInDeck => false;

        protected override Game ProcessEvent(Game game, TEvent e, TContext source)
        {
            if (EventFilter?.Includes(e, source, game) ?? true)
            {
                // At this point the trigger is "firing", but still might do nothing because of the condition.
                // (Not sure if this will matter here, but some triggers can only fire once, e.g. next card played.)
                if (Condition?.IsMet(e, source, game) ?? true)
                {
                    var effect = EffectBuilder.Build(e, source, game);
                    return effect.Apply(game);
                }
            }

            return game;
        }
    }

    public record TriggeredAbilityDiscardedOrDestroyed<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter,
        ICondition<TEvent, TContext>? Condition = null
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter, Condition)
        where TEvent : Event
    {
        public override bool WhenDiscardedOrDestroyed => true;
    }

    public record TriggeredAbilityInHand<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter,
        ICondition<TEvent, TContext>? Condition = null
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter, Condition)
        where TEvent : Event
    {
        public override bool WhenInHand => true;
    }

    public record TriggeredAbilityInDeck<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter,
        ICondition<TEvent, TContext>? Condition = null
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter, Condition)
        where TEvent : Event
    {
        public override bool WhenInDeck => true;
    }

    public record TriggeredAbilityInHandOrDeck<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter,
        ICondition<TEvent, TContext>? Condition = null
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter, Condition)
        where TEvent : Event
    {
        public override bool WhenInHand => true;
        public override bool WhenInDeck => true;
    }
}
