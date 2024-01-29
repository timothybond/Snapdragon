namespace Snapdragon
{
    public interface ITriggerBuilder<T>
    {
        ITrigger Build(GameState game, T source);
    }
}
