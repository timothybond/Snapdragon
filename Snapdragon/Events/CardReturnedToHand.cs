namespace Snapdragon.Events
{
    public record CardReturnedToHand(ICard Card, int Turn)
        : CardEvent(EventType.CardReturnedToHand, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Returned To Hand ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}
