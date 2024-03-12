namespace Snapdragon
{
    public interface ITriggeredAbility<TSource>
    {
        Game ProcessEvent(Game game, Event e, TSource source);
    }
}
