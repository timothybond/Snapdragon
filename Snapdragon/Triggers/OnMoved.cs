using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that fires when the source card is moved.
    /// </summary>
    public record OnMoved : ITrigger<Card>
    {
        public bool IsMet(Event e, Game game, Card source)
        {
            return e is CardMovedEvent cardMoved && cardMoved.Card.Id == source.Id;
        }
    }
}
