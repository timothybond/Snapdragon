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
            var effects = new List<IEffect>();
            var nextGameState = game;

            // Sometimes we need to check the modified context, e.g.
            // if we're filling out random locations with cards and
            // we need to choose a still-open space.
            for (var i = 0; i < count; i++)
            {
                var nextEffect = EffectBuilder.Build(context, nextGameState);
                nextGameState = nextEffect.Apply(game);

                effects.Add(nextEffect);
            }

            return new AndEffect(effects);
        }
    }

    /// <summary>
    /// An <see cref="IEffectBuilder{TEvent, TContext}"/> that repeats the given effect a calculated number of times.
    /// </summary>
    public record RepeatEffectBuilder<TEvent, TContext>(
        IEffectBuilder<TEvent, TContext> EffectBuilder,
        ICalculation<TEvent, TContext> Times
    ) : IEffectBuilder<TEvent, TContext>
        where TContext : class
        where TEvent : Event
    {
        public IEffect Build(TEvent e, TContext context, Game game)
        {
            var count = Times.GetValue(e, context, game);
            var effects = new List<IEffect>();
            var nextGameState = game;

            // Sometimes we need to check the modified context, e.g.
            // if we're filling out random locations with cards and
            // we need to choose a still-open space.
            for (var i = 0; i < count; i++)
            {
                var nextEffect = EffectBuilder.Build(e, context, nextGameState);
                nextGameState = nextEffect.Apply(game);

                effects.Add(nextEffect);
            }

            return new AndEffect(effects);
        }
    }
}
