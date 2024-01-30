namespace Snapdragon.Triggers
{
    public record OnEventType(EventType Type) : ITrigger
    {
        public bool IsMet(Event e, Game game)
        {
            return e.Type == this.Type;
        }
    }
}
