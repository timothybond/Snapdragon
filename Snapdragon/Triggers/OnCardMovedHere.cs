using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    public record OnCardMovedHere : ITrigger<Card>
    {
        public bool IsMet(Event e, Game game, Card source)
        {
            return e is CardMovedEvent cardMoved && cardMoved.To == source.Column;
        }
    }
}
