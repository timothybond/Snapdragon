namespace Snapdragon.Sensors
{
    public interface ITriggeredSensorAbilityBuilder<T>
    {
        TriggeredSensorAbility<T> Build(Game game, T source);
    }
}
