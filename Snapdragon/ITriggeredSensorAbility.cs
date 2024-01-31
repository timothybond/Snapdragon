namespace Snapdragon
{
    public interface ITriggeredSensorAbility
    {
        Game ProcessEvent(Game game, Event e);
    }
}
