namespace Snapdragon.Events
{
    public record CardReturnedToPlay(ICard Card, Column Column, int Turn)
        : CardEvent(EventType.CardReturnedToPlay, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Returned To Play ({Card.Side}, {Column}): {Card.Name} ({Card.Id})";
        }
    }
}
