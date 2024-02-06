namespace Snapdragon.Sensors
{
    public interface ISensorMoveAbilityBuilder<T>
    {
        IMoveAbility<Sensor<T>> Build(Game game, T source);
    }
}
