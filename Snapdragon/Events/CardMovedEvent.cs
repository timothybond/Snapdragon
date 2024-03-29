namespace Snapdragon.Events
{
    public record CardMovedEvent(int Turn, ICardInstance Card, Column From, Column To)
        : CardEvent(EventType.CardMoved, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Moved: {Card.Name} ({Card.Id}) from {From} to {To}.";
        }
    }
}
