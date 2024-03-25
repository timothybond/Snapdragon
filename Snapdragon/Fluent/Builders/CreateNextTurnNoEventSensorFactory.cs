using Snapdragon.Events;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent.Builders
{
    public record CreateNextTurnNoEventSensorFactory<TFilteredEventType>()
        : IResultFactory<
            OnReveal<Card>,
            TFilteredEventType,
            Sensor<Card>,
            IEffectBuilder<TurnEndedEvent, Sensor<Card>>
        >
        where TFilteredEventType : Event
    {
        public OnReveal<Card> Build(
            IEffectBuilder<TurnEndedEvent, Sensor<Card>> outcome,
            IEventFilter<TFilteredEventType, Sensor<Card>>? eventFilter = null,
            ICondition<TFilteredEventType, Sensor<Card>>? condition = null
        )
        {
            var trigger = When
                .Sensor.InPlayAnd<TurnEndedEvent>()
                .Where(new TurnAfterReveal<Sensor<Card>>())
                .If.NoPastEvent()
                .OfType<TFilteredEventType>()
                .Where(eventFilter.And(new CurrentTurnFilter()))
                .Build(outcome.And(new DestroySensorBuilder()));

            return new CardRevealed().Build(new CreateTriggeredSensorBuilder(trigger));
        }
    }
}
