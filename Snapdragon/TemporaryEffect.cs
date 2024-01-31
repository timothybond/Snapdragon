namespace Snapdragon
{
    public record TemporaryEffect<T>(
        int Id,
        Column Column,
        Side Side,
        T Source,
        ITriggeredEffectAbility? Ability
    ) { }
}
