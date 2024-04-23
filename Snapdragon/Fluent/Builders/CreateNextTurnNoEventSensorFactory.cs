using Snapdragon.Events;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent.Builders
{
    public record CreateNextTurnNoEventSensorFactory<TFilteredEventType>()
        : IResultFactory<
            OnReveal<ICard>,
            TFilteredEventType,
            Sensor<ICard>,
            IEffectBuilder<TurnEndedEvent, Sensor<ICard>>
        >
        where TFilteredEventType : Event
    {
        public OnReveal<ICard> Build(
            IEffectBuilder<TurnEndedEvent, Sensor<ICard>> outcome,
            IEventFilter<TFilteredEventType, Sensor<ICard>>? eventFilter = null,
            ICondition<TFilteredEventType, Sensor<ICard>>? condition = null
        )
        {
            var trigger = When
                .Sensor.InPlayAnd<TurnEndedEvent>()
                .Where(new TurnAfterReveal<Sensor<ICard>>())
                .If.NoPastEvent()
                .OfType<TFilteredEventType>()
                .Where(eventFilter.And(new CurrentTurnFilter()))
                .Then(outcome.And(new DestroySensorBuilder()));

            return new CardRevealed().Then(new CreateTriggeredSensorBuilder(trigger));
        }
    }
}
