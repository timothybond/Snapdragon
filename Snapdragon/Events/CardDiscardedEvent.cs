namespace Snapdragon.Events
{
    public record CardDiscardedEvent(int Turn, ICard Card)
        : Event(EventType.CardDiscarded, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Discarded: {Card.Name} ({Card.Id}), Side {Card.Side}.";
        }
    }
}
