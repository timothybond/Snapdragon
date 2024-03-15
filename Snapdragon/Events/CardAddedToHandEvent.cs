namespace Snapdragon.Events
{
    public record CardAddedToHandEvent(ICard Card, int Turn)
        : Event(EventType.CardAddedToHand, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Added To Hand: {Card.Name} ({Card.Id}), Side {Card.Side}.";
        }
    }
}
