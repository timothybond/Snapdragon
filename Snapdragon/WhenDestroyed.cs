using Snapdragon.Events;

namespace Snapdragon
{
    /// <summary>
    /// A triggered ability that fires when the given card is destroyed.
    /// </summary>
    public record WhenDestroyed(
        ISourceTriggeredEffectBuilder<ICard, CardDestroyedFromPlayEvent> EffectBuilder
    ) : BaseTriggeredCardAbility<CardDestroyedFromPlayEvent>
    {
        public override bool InHand => false;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => true;

        protected override Game ProcessEvent(Game game, CardDestroyedFromPlayEvent e, ICard source)
        {
            if (e.Card.Id == source.Id)
            {
                var effect = EffectBuilder.Build(game, e, source);
                return effect.Apply(game);
            }
            else
            {
                return game;
            }
        }
    }
}
