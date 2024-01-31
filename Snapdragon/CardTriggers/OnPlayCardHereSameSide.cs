using Snapdragon.Events;

namespace Snapdragon.CardTriggers
{
    /// <summary>
    /// Triggers when another <see cref="Card"/> is played at the same location on the same side.
    /// </summary>
    public record OnPlayCardHereSameSide : ITrigger<Card>
    {
        public bool IsMet(Event e, Game game, Card source)
        {
            if (e.Type != EventType.CardRevealed)
            {
                return false;
            }

            if (e is CardRevealedEvent cardRevealed)
            {
                return cardRevealed.Card.Column == source.Column
                    && cardRevealed.Card.Side == source.Side
                    && cardRevealed.Card.Id != source.Id;
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
