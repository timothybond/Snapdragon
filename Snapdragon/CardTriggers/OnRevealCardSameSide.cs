using Snapdragon.Events;

namespace Snapdragon.CardTriggers
{
    /// <summary>
    /// Triggers when another <see cref="CardInstance"/> is played on the same side, at any location.
    /// </summary>
    public record OnRevealCardSameSide : ITrigger<ICard, CardRevealedEvent>
    {
        public bool IsMet(CardRevealedEvent e, Game game, ICard source)
        {
            return e.Card.Side == source.Side && e.Card.Id != source.Id;
        }
    }
}
