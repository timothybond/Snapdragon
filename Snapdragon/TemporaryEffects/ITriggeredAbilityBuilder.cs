namespace Snapdragon.TemporaryEffects
{
    public interface ITriggeredAbilityBuilder<T>
    {
        TriggeredAbility<T> Build(Game game, T source);
    }
}
