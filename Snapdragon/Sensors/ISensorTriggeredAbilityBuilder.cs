namespace Snapdragon.Sensors
{
    public interface ISensorTriggeredAbilityBuilder<T>
    {
        ISensorTriggeredAbility Build(Game game, T source);
    }
}
