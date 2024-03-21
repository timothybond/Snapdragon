using Snapdragon.Events;
using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public static class If
    {
        public static Builder<
            OnReveal<Card>,
            TFilteredEventType,
            Sensor<Card>,
            IEffectBuilder<TurnEndedEvent, Sensor<Card>>
        > NextTurnEvent<TFilteredEventType>()
            where TFilteredEventType : Event
        {
            return new Builder<
                OnReveal<Card>,
                TFilteredEventType,
                Sensor<Card>,
                IEffectBuilder<TurnEndedEvent, Sensor<Card>>
            >(new CreateNextTurnEventSensorFactory<TFilteredEventType>());
        }

        public static Builder<
            OnReveal<Card>,
            TFilteredEventType,
            Sensor<Card>,
            IEffectBuilder<TurnEndedEvent, Sensor<Card>>
        > NoNextTurnEvent<TFilteredEventType>()
            where TFilteredEventType : Event
        {
            return new Builder<
                OnReveal<Card>,
                TFilteredEventType,
                Sensor<Card>,
                IEffectBuilder<TurnEndedEvent, Sensor<Card>>
            >(new CreateNextTurnNoEventSensorFactory<TFilteredEventType>());
        }
    }
}
