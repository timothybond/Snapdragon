namespace Snapdragon.Events
{
    public record CardRevealedEvent(int Turn, ICard Card) : Event(EventType.CardRevealed, Turn)
    {
        public override string ToString()
        {
            return $"Card Revealed: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
