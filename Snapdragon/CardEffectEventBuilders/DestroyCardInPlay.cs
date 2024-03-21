using Snapdragon.TriggeredAbilities;

namespace Snapdragon.CardEffectEventBuilders
{
    public record DestroyCardInPlay : ICardEventEffectBuilder
    {
        public IEffect Build(CardEvent e, Game game)
        {
            return new Effects.DestroyCardInPlay(e.Card);
        }
    }
}
