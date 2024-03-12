using Snapdragon.Effects;
using Snapdragon.TriggeredAbilities;

namespace Snapdragon.CardEffectEventBuilders
{
    public record DestroyCardInPlay : ICardEventEffectBuilder
    {
        public IEffect Build(ICardEvent e)
        {
            return new Effects.DestroyCardInPlay(e.Card);
        }
    }
}
