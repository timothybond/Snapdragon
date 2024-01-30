namespace Snapdragon
{
    public record TemporaryEffect<T>(
        int Id,
        Column Column,
        Side Side,
        T Source,
        ITriggeredAbility<TemporaryEffect<T>>? Ability) { }
}
