using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public class DeleteOnTrigger<TEvent>(
        ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent> EffectBuilder
    ) : ISourceTriggeredEffectBuilder<Sensor<Card>, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Sensor<Card> source)
        {
            var baseEffect = EffectBuilder.Build(game, e, source);
            var deleteEffect = new DeleteTemporaryCardEffect(source);
            return new AndEffect(baseEffect, deleteEffect);
        }
    }
}
