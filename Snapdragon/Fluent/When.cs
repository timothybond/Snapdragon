using Snapdragon.Events;
using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent
{
    public static class When
    {
        public static TriggerBuilder<TEvent, ICard> InPlayAnd<TEvent>()
            where TEvent : Event
        {
            return new TriggerBuilder<TEvent, ICard>();
        }

        public static TriggerBuilder<TEvent, Location> RevealedAnd<TEvent>()
            where TEvent : Event
        {
            return new TriggerBuilder<TEvent, Location>();
        }

        public static DiscardedTriggerBuilder Discarded => new DiscardedTriggerBuilder();

        public static DestroyedTriggerBuilder Destroyed => new DestroyedTriggerBuilder();

        public static DiscardedOrDestroyedTriggerBuilder DiscardedOrDestroyed =>
            new DiscardedOrDestroyedTriggerBuilder();

        public static OnReveal<Card> NextCardRevealed(
            IEffectBuilder<CardRevealedEvent, Sensor<Card>> outcome
        )
        {
            var trigger = Sensor
                .InPlayAnd<CardRevealedEvent>()
                .Where(new NextRevealedCard<Sensor<Card>>(new SourceCard(), new SourceCard()))
                .Build(outcome);

            return new CardRevealed().Build(new CreateTriggeredSensorBuilder(trigger));
        }

        public static class CardSensor
        {
            public static TriggerBuilder<TEvent, Sensor<Card>> InPlayAnd<TEvent>()
                where TEvent : Event
            {
                return new TriggerBuilder<TEvent, Sensor<Card>>();
            }
        }

        public static class Sensor
        {
            public static TriggerBuilder<TEvent, Sensor<Card>> InPlayAnd<TEvent>()
                where TEvent : Event
            {
                return new TriggerBuilder<TEvent, Sensor<Card>>();
            }
        }
    }
}
