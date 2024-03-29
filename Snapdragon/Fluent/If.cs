using Snapdragon.Events;
using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public static class If
    {
        public static Builder<
            OnReveal<ICard>,
            TFilteredEventType,
            Sensor<ICard>,
            IEffectBuilder<TurnEndedEvent, Sensor<ICard>>
        > NextTurnEvent<TFilteredEventType>()
            where TFilteredEventType : Event
        {
            return new Builder<
                OnReveal<ICard>,
                TFilteredEventType,
                Sensor<ICard>,
                IEffectBuilder<TurnEndedEvent, Sensor<ICard>>
            >(new CreateNextTurnEventSensorFactory<TFilteredEventType>());
        }

        public static Builder<
            OnReveal<ICard>,
            TFilteredEventType,
            Sensor<ICard>,
            IEffectBuilder<TurnEndedEvent, Sensor<ICard>>
        > NoNextTurnEvent<TFilteredEventType>()
            where TFilteredEventType : Event
        {
            return new Builder<
                OnReveal<ICard>,
                TFilteredEventType,
                Sensor<ICard>,
                IEffectBuilder<TurnEndedEvent, Sensor<ICard>>
            >(new CreateNextTurnNoEventSensorFactory<TFilteredEventType>());
        }
    }
}
