using Snapdragon.Events;

namespace Snapdragon
{
    /// <summary>
    /// A triggered ability that fires when the given card is discarded.
    /// </summary>
    public record WhenDiscarded(
        ISourceTriggeredEffectBuilder<Card, CardDiscardedEvent> EffectBuilder
    ) : BaseTriggeredAbility<Card, CardDiscardedEvent>
    {
        public override bool InHand => false;
        public override bool InDeck => false;
        public override bool DiscardedOrDestroyed => true;

        protected override Game ProcessEvent(Game game, CardDiscardedEvent e, Card source)
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
