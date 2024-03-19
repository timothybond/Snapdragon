using Snapdragon.Fluent.EffectBuilders;

namespace Snapdragon.Fluent
{
    public static class EffectBuilderExtensions
    {
        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TContext}"/> that performs two different effects, in order.
        /// </summary>
        public static AndEffectBuilder<TContext> And<TContext>(
            this IEffectBuilder<TContext> first,
            IEffectBuilder<TContext> second
        )
        {
            return new AndEffectBuilder<TContext>(first, second);
        }

        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TContext}"/> that performs one effect the given number of times.
        /// </summary>
        public static AndEffectBuilder<TContext> Times<TContext>(
            this IEffectBuilder<TContext> baseEffectBuilder,
            int times
        )
        {
            return new AndEffectBuilder<TContext>(Enumerable.Repeat(baseEffectBuilder, times));
        }
    }
}
