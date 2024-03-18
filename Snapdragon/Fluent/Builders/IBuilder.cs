namespace Snapdragon.Fluent.Builders
{
    public interface IBuilder<TResult, TContext>
    {
        TResult Build(IEffectBuilder<TContext> effectBuilder);
    }
}
