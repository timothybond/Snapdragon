namespace Snapdragon.Events
{
    public record CardDrawnEvent(int Turn, CardInstance Card) : Event(EventType.CardDrawn, Turn)
    {
        public override string ToString()
        {
            return $"Card Drawn ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}
