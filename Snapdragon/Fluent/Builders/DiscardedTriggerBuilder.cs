using Snapdragon.Events;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent.Builders
{
    public record DiscardedTriggerBuilder
        : IBuilder<ITriggeredAbility<ICard>, ICard, IEffectBuilder<CardDiscardedEvent, ICard>>
    {
        public ITriggeredAbility<ICard> Build(IEffectBuilder<CardDiscardedEvent, ICard> outcome)
        {
            return new TriggeredAbilityDiscardedOrDestroyed<CardDiscardedEvent, ICard>(
                outcome,
                new Self()
            );
        }
    }

    public record DestroyedTriggerBuilder
        : IBuilder<
            ITriggeredAbility<ICard>,
            ICard,
            IEffectBuilder<CardDestroyedFromPlayEvent, ICard>
        >
    {
        public ITriggeredAbility<ICard> Build(
            IEffectBuilder<CardDestroyedFromPlayEvent, ICard> outcome
        )
        {
            return new TriggeredAbilityDiscardedOrDestroyed<CardDestroyedFromPlayEvent, ICard>(
                outcome,
                new Self()
            );
        }
    }

    public record DiscardedOrDestroyedTriggerBuilder
        : IBuilder<ITriggeredAbility<ICard>, ICard, IEffectBuilder<Event, ICard>>
    {
        public ITriggeredAbility<ICard> Build(IEffectBuilder<Event, ICard> outcome)
        {
            return new TriggeredAbilityDiscardedOrDestroyed<Event, ICard>(
                outcome,
                new SelfDiscardedOrDestroyed()
            );
        }

        private record SelfDiscardedOrDestroyed() : IEventFilter<Event, ICard>
        {
            public bool Includes(Event e, ICard context, Game game)
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
