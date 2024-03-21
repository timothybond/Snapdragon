namespace Snapdragon
{
    public record Sensor<T>(
        long Id,
        Column Column,
        Side Side,
        T Source,
        ITriggeredAbility<Sensor<T>>? TriggeredAbility,
        IMoveAbility<Sensor<T>>? MoveAbility = null,
        int? TurnRevealed = null
    ) : IObjectWithColumn, IRevealableObject, IObjectWithSide
    {
        Column? IObjectWithPossibleColumn.Column => Column;
    }
}
