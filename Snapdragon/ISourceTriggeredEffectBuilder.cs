namespace Snapdragon
{
    public interface ISourceTriggeredEffectBuilder<in TSource, in TEvent>
    {
        IEffect Build(Game game, TEvent e, TSource source);
    }
}
