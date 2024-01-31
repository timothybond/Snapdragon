namespace Snapdragon
{
    public interface ITriggeredEffectAbility
    {
        Game ProcessEvent(Game game, Event e);
    }
}
