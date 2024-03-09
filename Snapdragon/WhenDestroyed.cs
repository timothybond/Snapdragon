using Snapdragon.Events;

namespace Snapdragon
{
    /// <summary>
    /// A triggered ability that fires when the given card is destroyed.
    /// </summary>
    public record WhenDestroyed(
        ISourceTriggeredEffectBuilder<CardInstance, CardDestroyedFromPlayEvent> EffectBuilder
    ) : BaseTriggeredAbility<CardInstance, CardDestroyedFromPlayEvent>
    {
        public override bool InHand => false;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => true;

        protected override Game ProcessEvent(Game game, CardDestroyedFromPlayEvent e, CardInstance source)
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
