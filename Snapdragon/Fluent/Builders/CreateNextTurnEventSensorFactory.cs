using Snapdragon.Events;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent.Builders
{
    public record CreateNextTurnEventSensorFactory<TFilteredEventType>(
        IEffectBuilder<TurnEndedEvent, Sensor<Card>> EffectBuilder,
        IEventFilter<TFilteredEventType, Sensor<Card>>? EventFilter
    ) : IEffectBuilder<Card>
    {
        public IEffect Build(Card context, Game game)
        {
            var trigger = When
                .Sensor.InPlayAnd<TurnEndedEvent>()
                .Where(new SpecificTurnFilter(game.Turn + 1))
                .If.PastEvent()
                .OfType<TFilteredEventType>()
                .Where(EventFilter)
                .Build(EffectBuilder.And(new DestroySensorBuilder()));

            throw new NotImplementedException();
        }
    }
}
