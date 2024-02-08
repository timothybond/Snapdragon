using Snapdragon.Events;

namespace Snapdragon.CardTriggers
{
    /// <summary>
    /// Triggers when another <see cref="Card"/> is played on the same side, at any location.
    /// </summary>
    public record OnRevealCardSameSide : ITrigger<Card, CardRevealedEvent>
    {
        public bool IsMet(CardRevealedEvent e, Game game, Card source)
        {
            return e.Card.Side == source.Side && e.Card.Id != source.Id;
        }
    }
}
