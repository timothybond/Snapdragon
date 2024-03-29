namespace Snapdragon.Events
{
    public record CardRevealedEvent(int Turn, ICardInstance Card)
        : CardEvent(EventType.CardRevealed, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Revealed: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
