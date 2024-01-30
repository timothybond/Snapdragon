using Snapdragon.Effects;

namespace Snapdragon.TemporaryEffects
{
    public class DeleteOnTrigger(IEffectBuilder<TemporaryEffect<Card>> EffectBuilder) : IEffectBuilder<TemporaryEffect<Card>>
    {
        public IEffect Build(GameState game, TemporaryEffect<Card> source)
        {
            var baseEffect = EffectBuilder.Build(game, source);
            var deleteEffect = new DeleteTemporaryCardEffect(source);
            return new AndEffect(baseEffect, deleteEffect);
        }
    }
}
