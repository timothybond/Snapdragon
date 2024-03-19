﻿using Snapdragon.TriggeredAbilities;

namespace Snapdragon.Fluent
{
    public record TriggeredAbility<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter
    ) : BaseTriggeredAbility<TContext, TEvent>
        where TEvent : Event
    {
        public virtual bool WhenDiscardedOrDestroyed => false;
        public virtual bool WhenInHand => false;
        public virtual bool WhenInDeck => false;

        protected override Game ProcessEvent(Game game, TEvent e, TContext source)
        {
            if (EventFilter?.Includes(e, source, game) ?? true)
            {
                var effect = EffectBuilder.Build(e, source, game);
                return effect.Apply(game);
            }
            else
            {
                return game;
            }
        }
    }

    public record TriggeredAbilityDiscardedOrDestroyed<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter)
        where TEvent : Event
    {
        public override bool WhenDiscardedOrDestroyed => true;
    }

    public record TriggeredAbilityInHand<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter)
        where TEvent : Event
    {
        public override bool WhenInHand => true;
    }

    public record TriggeredAbilityInDeck<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter)
        where TEvent : Event
    {
        public override bool WhenInDeck => true;
    }

    public record TriggeredAbilityInHandOrDeck<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        IEventFilter<TEvent, TContext>? EventFilter
    ) : TriggeredAbility<TEvent, TContext>(EffectBuilder, EventFilter)
        where TEvent : Event
    {
        public override bool WhenInHand => true;
        public override bool WhenInDeck => true;
    }
}
