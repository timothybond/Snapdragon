namespace Snapdragon.Events
{
    public record CardAddedToLocationEvent(ICard Card, Column Column, int Turn)
        : Event(EventType.CardAddedToLocation, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Added: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Column}.";
        }
    }
}
