namespace Snapdragon.Events
{
    public record CardReturnedToHand(ICard Card, int Turn)
        : Event(EventType.CardReturnedToHand, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Returned To Hand ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}
