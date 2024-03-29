using Snapdragon.Events;

namespace Snapdragon
{
    /// <summary>
    /// A triggered ability that fires when the given card is discarded.
    /// </summary>
    public record WhenDiscarded(
        ISourceTriggeredEffectBuilder<ICardInstance, CardDiscardedEvent> EffectBuilder
    ) : BaseTriggeredCardAbility<CardDiscardedEvent>
    {
        public override bool WhenInHand => false;
        public override bool WhenInDeck => false;
        public override bool WhenDiscardedOrDestroyed => true;

        protected override Game ProcessEvent(Game game, CardDiscardedEvent e, ICardInstance source)
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
