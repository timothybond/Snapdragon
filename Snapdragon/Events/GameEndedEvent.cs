namespace Snapdragon.Events
{
    public record GameEndedEvent(int Turn, Side? Winner) : Event(EventType.GameEnded, Turn)
    {
        public override string ToString()
        {
            return $"Game ended - winner: {Winner}.";
        }
    }
}
