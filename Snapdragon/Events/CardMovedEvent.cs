namespace Snapdragon.Events
{
    public record CardMovedEvent(int Turn, Card Card, Column From, Column To)
        : Event(EventType.CardMoved, Turn)
    {
        public override string ToString()
        {
            return $"Card Moved: {Card.Name} ({Card.Id}) from {From} to {To}.";
        }
    }
}
