namespace Snapdragon.Events
{
    public record GameEndedEvent(int Turn, Side? Winner) : Event(EventType.GameEnded)
    {
        public override string ToString()
        {
            return $"Game ended - winner: {Winner}.";
        }
    }
}
