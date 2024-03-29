using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public class DeleteOnTrigger<TEvent>(
        ISourceTriggeredEffectBuilder<Sensor<ICard>, TEvent> EffectBuilder
    ) : ISourceTriggeredEffectBuilder<Sensor<ICard>, TEvent>
    {
        public IEffect Build(Game game, TEvent e, Sensor<ICard> source)
        {
            var baseEffect = EffectBuilder.Build(game, e, source);
            var deleteEffect = new DeleteTemporaryCardEffect(source);
            return new AndEffect(baseEffect, deleteEffect);
        }
    }
}
