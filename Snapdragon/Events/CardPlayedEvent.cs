namespace Snapdragon.Events
{
    public record CardPlayedEvent(int Turn, ICard Card)
        : CardEvent(EventType.CardPlayed, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Played: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
