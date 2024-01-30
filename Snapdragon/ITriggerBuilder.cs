namespace Snapdragon
{
    public interface ITriggerBuilder<T>
    {
        ITrigger Build(Game game, T source);
    }
}
