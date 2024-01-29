namespace Snapdragon
{
    public interface IEffectBuilder<T>
    {
        IEffect Build(GameState game, T source);
    }
}
