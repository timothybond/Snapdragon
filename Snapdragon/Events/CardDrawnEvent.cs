namespace Snapdragon.Events
{
    public record CardDrawnEvent(int Turn, ICard Card)
        : Event(EventType.CardDrawn, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card Drawn ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}
