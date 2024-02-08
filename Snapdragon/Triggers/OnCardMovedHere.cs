using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    public record OnCardMovedHere : ITrigger<Card, CardMovedEvent>
    {
        public bool IsMet(CardMovedEvent e, Game game, Card source)
        {
            return e.To == source.Column;
        }
    }
}
