using Snapdragon.Events;

namespace Snapdragon.CardTriggers
{
    /// <summary>
    /// Triggers when another <see cref="CardInstance"/> is played at the same location on the same side.
    /// </summary>
    public record OnRevealCardHereSameSide : ITrigger<ICardInstance, CardRevealedEvent>
    {
        public bool IsMet(CardRevealedEvent e, Game game, ICardInstance source)
        {
            return e.Card.Column == source.Column
                && e.Card.Side == source.Side
                && e.Card.Id != source.Id;
        }
    }
}
