namespace Snapdragon.Events
{
    public record CardDrawnEvent(int Turn, ICardInstance Card) : CardEvent(EventType.CardDrawn, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card Drawn ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}
