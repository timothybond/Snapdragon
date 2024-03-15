namespace Snapdragon
{
    public record Sensor<T>(
        long Id,
        Column Column,
        Side Side,
        T Source,
        ISensorTriggeredAbility? TriggeredAbility,
        IMoveAbility<Sensor<T>>? MoveAbility = null
    ) : IObjectWithColumn
    {
        Column? IObjectWithPossibleColumn.Column => Column;
    }
}
