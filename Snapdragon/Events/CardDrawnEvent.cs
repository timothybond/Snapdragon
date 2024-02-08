﻿namespace Snapdragon.Events
{
    public record CardDrawnEvent(int Turn, Card Card) : Event(EventType.CardDrawn, Turn)
    {
        public override string ToString()
        {
            return $"Card Drawn ({Card.Side}): {Card.Name} ({Card.Id})";
        }
    }
}