namespace Snapdragon.Events
{
    public record LocationRevealedEvent(int Turn, Location Location)
        : Event(EventType.LocationRevealed, Turn)
    {
        public override string ToString()
        {
            return $"Location Revealed: {Location.Definition.Name} ({Location.Column}).";
        }
    }
}
