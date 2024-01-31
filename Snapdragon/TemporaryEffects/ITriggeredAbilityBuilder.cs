namespace Snapdragon.TemporaryEffects
{
    public interface ITriggeredAbilityBuilder<T>
    {
        TriggeredEffectAbility<T> Build(Game game, T source);
    }
}
