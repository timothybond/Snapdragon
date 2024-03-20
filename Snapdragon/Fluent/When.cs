using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public static class When
    {
        public static TriggerBuilder<TEvent, Card> InPlayAnd<TEvent>()
            where TEvent : Event
        {
            return new TriggerBuilder<TEvent, Card>();
        }

        public static DiscardedTriggerBuilder Discarded => new DiscardedTriggerBuilder();

        public static DestroyedTriggerBuilder Destroyed => new DestroyedTriggerBuilder();

        public static DiscardedOrDestroyedTriggerBuilder DiscardedOrDestroyed =>
            new DiscardedOrDestroyedTriggerBuilder();

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
