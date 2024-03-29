namespace Snapdragon
{
    public abstract record CardEvent(EventType Type, int Turn, ICardInstance Card) : Event(Type, Turn) { }
}
