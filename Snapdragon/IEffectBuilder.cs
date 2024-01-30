namespace Snapdragon
{
    public interface IEffectBuilder<T>
    {
        IEffect Build(Game game, T source);
    }
}
