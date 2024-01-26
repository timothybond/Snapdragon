namespace Snapdragon.Events
{
    public record TurnStartedEvent(int Turn) : Event(EventType.TurnStarted)
    {
        public override string ToString()
        {
            return $"Turn {Turn} started.";
        }
    }
}
