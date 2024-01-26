namespace Snapdragon.Events
{
    public record CardPlayedEvent(int Turn, Card Card) : Event(EventType.CardPlayed)
    {
        public override string ToString()
        {
            return $"Card Played: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
