namespace Snapdragon.Sensors
{
    public interface ISensorTriggeredAbilityBuilder<TSource, TEvent>
        where TEvent : Event
    {
        ISensorTriggeredAbility Build(Game game, TSource source);
    }
}
