using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// An <see cref="IEffectBuilder{TContext}"/> that repeats the given effect a calculated number of times.
    /// </summary>
    public record RepeatEffectBuilder<TContext>(
        IEffectBuilder<TContext> EffectBuilder,
        ICalculation<TContext> Times
    ) : IEffectBuilder<TContext>
        where TContext : class
    {
        public IEffect Build(TContext context, Game game)
        {
            var count = Times.GetValue(context, game);
            return new AndEffect(
                Enumerable.Repeat(EffectBuilder, count).Select(eb => eb.Build(context, game))
            );
        }
    }
}
