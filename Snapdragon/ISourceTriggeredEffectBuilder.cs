namespace Snapdragon
{
    public interface ISourceTriggeredEffectBuilder<T>
    {
        IEffect Build(Game game, Event e, T source);
    }
}
