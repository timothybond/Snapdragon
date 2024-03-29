using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    public record OnCardMovedHere : ITrigger<ICardInstance, CardMovedEvent>
    {
        public bool IsMet(CardMovedEvent e, Game game, ICardInstance source)
        {
            return e.To == source.Column;
        }
    }
}
