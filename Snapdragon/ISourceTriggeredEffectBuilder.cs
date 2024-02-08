namespace Snapdragon
{
    public interface ISourceTriggeredEffectBuilder<TSource, TEvent>
    {
        IEffect Build(Game game, TEvent e, TSource source);
    }
}
