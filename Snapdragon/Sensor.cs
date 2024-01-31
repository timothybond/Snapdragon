namespace Snapdragon
{
    public record Sensor<T>(
        int Id,
        Column Column,
        Side Side,
        T Source,
        ITriggeredSensorAbility? Ability
    )
    { }
}
