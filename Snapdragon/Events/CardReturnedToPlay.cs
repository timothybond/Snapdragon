namespace Snapdragon.Events
{
    public record CardReturnedToPlay(ICard Card, Column Column, int Turn)
        : Event(EventType.CardReturnedToPlay, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Returned To Play ({Card.Side}, {Column}): {Card.Name} ({Card.Id})";
        }
    }
}
