namespace Snapdragon.Events
{
    public record CardSwitchedSidesEvent(ICard Card, int Turn)
        : CardEvent(EventType.CardSwitchedSides, Turn, Card)
    {
        public override string ToString()
        {
            return $"Card {Card.Name} ({Card.Id}) switched sides from {Card.Side.Other()} to {Card.Side}.";
        }
    }
}
