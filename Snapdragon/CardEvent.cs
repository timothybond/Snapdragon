namespace Snapdragon
{
    public abstract record CardEvent(EventType Type, int Turn, ICard Card) : Event(Type, Turn) { }
}
