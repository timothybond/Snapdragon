namespace Snapdragon.Events
{
    public record TurnStartedEvent(int Turn) : Event(EventType.TurnStarted, Turn)
    {
        public override string ToString()
        {
            return $"Turn {Turn} started.";
        }
    }
}
