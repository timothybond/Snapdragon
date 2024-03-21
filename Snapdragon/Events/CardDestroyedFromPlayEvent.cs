namespace Snapdragon.Events
{
    public record CardDestroyedFromPlayEvent(int Turn, ICard Card)
        : CardEvent(EventType.CardDestroyedFromPlay, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Destroyed: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
