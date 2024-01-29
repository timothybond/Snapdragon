namespace Snapdragon.Events
{
    public record TurnEndedEvent(int Turn) : Event(EventType.TurnEnded, Turn)
    {
        public override string ToString()
        {
            return $"Turn {Turn} ended.";
        }
    }
}
