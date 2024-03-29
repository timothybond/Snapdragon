namespace Snapdragon.Events
{
    public record CardAddedToHandEvent(ICardInstance Card, int Turn)
        : CardEvent(EventType.CardAddedToHand, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Added To Hand: {Card.Name} ({Card.Id}), Side {Card.Side}.";
        }
    }
}
