namespace Snapdragon
{
    public record Sensor<T>(
        int Id,
        Column Column,
        Side Side,
        T Source,
        ISensorTriggeredAbility? TriggeredAbility,
        IMoveAbility<Sensor<T>>? MoveAbility = null
    ) : IObjectWithColumn
    {
        Column? IObjectWithColumn.Column => this.Column;
    }
}
