using Snapdragon.Events;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent.Builders
{
    public record DiscardedTriggerBuilder
        : IBuilder<ITriggeredAbility<ICardInstance>, ICardInstance, IEffectBuilder<CardDiscardedEvent, ICardInstance>>
    {
        public ITriggeredAbility<ICardInstance> Then(IEffectBuilder<CardDiscardedEvent, ICardInstance> outcome)
        {
            return new TriggeredAbilityDiscardedOrDestroyed<CardDiscardedEvent, ICardInstance>(
                outcome,
                new Self()
            );
        }
    }

    public record DestroyedTriggerBuilder
        : IBuilder<
            ITriggeredAbility<ICardInstance>,
            ICardInstance,
            IEffectBuilder<CardDestroyedFromPlayEvent, ICardInstance>
        >
    {
        public ITriggeredAbility<ICardInstance> Then(
            IEffectBuilder<CardDestroyedFromPlayEvent, ICardInstance> outcome
        )
        {
            return new TriggeredAbilityDiscardedOrDestroyed<CardDestroyedFromPlayEvent, ICardInstance>(
                outcome,
                new Self()
            );
        }
    }

    public record DiscardedOrDestroyedTriggerBuilder
        : IBuilder<ITriggeredAbility<ICardInstance>, ICardInstance, IEffectBuilder<Event, ICardInstance>>
    {
        public ITriggeredAbility<ICardInstance> Then(IEffectBuilder<Event, ICardInstance> outcome)
        {
            return new TriggeredAbilityDiscardedOrDestroyed<Event, ICardInstance>(
                outcome,
                new SelfDiscardedOrDestroyed()
            );
        }

        private record SelfDiscardedOrDestroyed() : IEventFilter<Event, ICardInstance>
        {
            public bool Includes(Event e, ICardInstance context, Game game)
            {
                return (
                        e is CardDestroyedFromPlayEvent cardDestroyed
                        && cardDestroyed.Card.Id == context.Id
                    )
                    || (
                        e is CardDiscardedEvent cardDiscarded && cardDiscarded.Card.Id == context.Id
                    );
            }
        }
    }
}
