namespace Snapdragon
{
    public interface ITriggerBuilder<TSource, TEvent>
    {
        ITrigger<TEvent> Build(Game game, TSource source);
    }
}
