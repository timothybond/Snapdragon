namespace Snapdragon.Sensors
{
    public interface ISensorTriggeredAbilityBuilder<TSource, TEvent>
        where TEvent : Event
    {
        ITriggeredAbility<Sensor<Card>> Build(Game game, TSource source);
    }
}
