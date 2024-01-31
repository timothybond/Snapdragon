using Snapdragon.Effects;

namespace Snapdragon.Sensors
{
    public class DeleteOnTrigger(IEffectBuilder<Sensor<Card>> EffectBuilder)
        : IEffectBuilder<Sensor<Card>>
    {
        public IEffect Build(Game game, Sensor<Card> source)
        {
            var baseEffect = EffectBuilder.Build(game, source);
            var deleteEffect = new DeleteTemporaryCardEffect(source);
            return new AndEffect(baseEffect, deleteEffect);
        }
    }
}
