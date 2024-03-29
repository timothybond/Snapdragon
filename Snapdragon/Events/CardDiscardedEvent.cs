namespace Snapdragon.Events
{
    public record CardDiscardedEvent(int Turn, ICardInstance Card)
        : CardEvent(EventType.CardDiscarded, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Discarded: {Card.Name} ({Card.Id}), Side {Card.Side}.";
        }
    }
}
