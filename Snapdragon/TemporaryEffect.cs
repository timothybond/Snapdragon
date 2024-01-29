namespace Snapdragon
{
    public record TemporaryEffect<T>(
        Column Column,
        Side Side,
        T Source,
        IAbility<T> Ability,
        int? ExpiresAfterTurn = null
    ) { }
}
