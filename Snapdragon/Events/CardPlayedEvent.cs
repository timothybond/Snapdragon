namespace Snapdragon.Events
{
    public record CardPlayedEvent(int Turn, CardInstance Card) : Event(EventType.CardPlayed, Turn)
    {
        public override string ToString()
        {
            return $"Card Played: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
