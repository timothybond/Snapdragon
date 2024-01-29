namespace Snapdragon.TemporaryEffects
{
    public interface ITriggeredAbilityBuilder<T>
    {
        TriggeredAbility<T> Build(GameState game, T source);
    }
}
