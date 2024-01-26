namespace Snapdragon.Events
{
    public record LocationRevealedEvent(int Turn, Location Location)
        : Event(EventType.LocationRevealed)
    {
        public override string ToString()
        {
            return $"Location Revealed: {Location.Name} ({Location.Column}).";
        }
    }
}
