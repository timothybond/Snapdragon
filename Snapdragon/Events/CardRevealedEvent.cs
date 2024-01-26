namespace Snapdragon.Events
{
    public record CardRevealedEvent(int Turn, Card Card) : Event(EventType.CardRevealed)
    {
        public override string ToString()
        {
            return $"Card Revealed: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
