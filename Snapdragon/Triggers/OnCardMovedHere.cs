using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    public record OnCardMovedHere : ITrigger<ICard, CardMovedEvent>
    {
        public bool IsMet(CardMovedEvent e, Game game, ICard source)
        {
            return e.To == source.Column;
        }
    }
}
