namespace Snapdragon.Events
{
    public record CardSwitchedSidesEvent(ICard Card, int Turn)
        : Event(EventType.CardSwitchedSides, Turn),
            ICardEvent
    {
        public override string ToString()
        {
            return $"Card {Card.Name} ({Card.Id}) switched sides from {Card.Side.Other()} to {Card.Side}.";
        }
    }
}
