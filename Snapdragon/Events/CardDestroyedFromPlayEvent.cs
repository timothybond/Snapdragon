namespace Snapdragon.Events
{
    public record CardDestroyedFromPlayEvent(int Turn, Card Card)
        : Event(EventType.CardDestroyedFromPlay, Turn)
    {
        public override string ToString()
        {
            return $"Card Destroyed: {Card.Name} ({Card.Id}), Side {Card.Side}, Location {Card.Column}.";
        }
    }
}
