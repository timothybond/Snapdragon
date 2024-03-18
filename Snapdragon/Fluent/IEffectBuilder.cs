namespace Snapdragon.Fluent
{
    public interface IEffectBuilder<TContext>
    {
        IEffect Build(TContext context, Game game);
    }
}
