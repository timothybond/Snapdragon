using Snapdragon.Events;

namespace Snapdragon
{
    /// <summary>
    /// A triggered ability that fires when the given card is discarded.
    /// </summary>
    public record WhenDestroyed(ISourceTriggeredEffectBuilder<Card> EffectBuilder)
        : ITriggeredAbility<Card>
    {
        public bool InHand => false;
        public bool InDeck => false;
        public bool DiscardedOrDestroyed => true;

        public Game ProcessEvent(Game game, Event e, Card source)
        {
            // TODO: Determine if there's any other cases that apply
            if (e.Type != EventType.CardDestroyedFromPlay)
            {
                return game;
            }

            if (e is CardDestroyedFromPlayEvent cardDestroyed)
            {
                if (cardDestroyed.Card.Id == source.Id)
                {
                    var effect = EffectBuilder.Build(game, e, source);
                    return effect.Apply(game);
                }
                else
                {
                    return game;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Event had type CardRevealed but was not of type CardRevealedEvent."
                );
            }
        }
    }
}
