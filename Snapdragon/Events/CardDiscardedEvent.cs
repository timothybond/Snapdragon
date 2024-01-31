namespace Snapdragon.Events
{
    public record CardDiscardedEvent(int Turn, Card Card) : Event(EventType.CardDiscarded, Turn)
    {
        public override string ToString()
        {
            return $"Card Discarded: {Card.Name} ({Card.Id}), Side {Card.Side}.";
        }
    }
}
