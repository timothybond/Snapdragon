namespace Snapdragon.Fluent
{
    public interface IEventFilter<TEvent>
    {
        bool Includes(TEvent e);
    }
}
