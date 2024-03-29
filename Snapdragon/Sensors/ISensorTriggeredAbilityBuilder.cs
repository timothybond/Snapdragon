namespace Snapdragon.Sensors
{
    public interface ISensorTriggeredAbilityBuilder<TSource, TEvent>
        where TEvent : Event
    {
        ITriggeredAbility<Sensor<ICard>> Build(Game game, TSource source);
    }
}
