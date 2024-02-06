using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public class DeleteOnTrigger(ISourceTriggeredEffectBuilder<Sensor<Card>> EffectBuilder)
        : ISourceTriggeredEffectBuilder<Sensor<Card>>
    {
        public IEffect Build(Game game, Event e, Sensor<Card> source)
        {
            var baseEffect = EffectBuilder.Build(game, e, source);
            var deleteEffect = new DeleteTemporaryCardEffect(source);
            return new AndEffect(baseEffect, deleteEffect);
        }
    }
}
