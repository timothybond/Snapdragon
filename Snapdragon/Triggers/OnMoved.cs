using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that fires when the source card is moved.
    /// </summary>
    public record OnMoved : ITrigger<Card, CardMovedEvent>
    {
        public bool IsMet(CardMovedEvent e, Game game, Card source)
        {
            return e.Card.Id == source.Id;
        }
    }
}
