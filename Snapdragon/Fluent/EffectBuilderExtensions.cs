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
        /// Gets an instance of <see cref="IEffectBuilder{TEvent, TContext}"/> that performs two different effects, in order.
        /// </summary>
        public static AndEffectBuilder<TEvent, TContext> And<TEvent, TContext>(
            this IEffectBuilder<TEvent, TContext> first,
            IEffectBuilder<TEvent, TContext> second
        )
            where TEvent : Event
        {
            return new AndEffectBuilder<TEvent, TContext>(first, second);
        }

        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TContext}"/> that performs one effect the given number of times.
        /// </summary>
        public static RepeatEffectBuilder<TContext> Times<TContext>(
            this IEffectBuilder<TContext> baseEffectBuilder,
            int times
        )
            where TContext : class
        {
            return new RepeatEffectBuilder<TContext>(baseEffectBuilder, new ConstantValue(times));
        }

        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TContext}"/> that performs one effect the given number of times.
        /// </summary>
        public static RepeatEffectBuilder<TContext> Times<TContext>(
            this IEffectBuilder<TContext> baseEffectBuilder,
            ICalculation<TContext> times
        )
            where TContext : class
        {
            return new RepeatEffectBuilder<TContext>(baseEffectBuilder, times);
        }

        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TEvent, TContext}"/> that performs one effect the given number of times.
        /// </summary>
        public static RepeatEffectBuilder<TEvent, TContext> Times<TEvent, TContext>(
            this IEffectBuilder<TEvent, TContext> baseEffectBuilder,
            int times
        )
            where TContext : class
            where TEvent : Event
        {
            return new RepeatEffectBuilder<TEvent, TContext>(
                baseEffectBuilder,
                new ConstantValue(times)
            );
        }

        /// <summary>
        /// Gets an instance of <see cref="IEffectBuilder{TEvent, TContext}"/> that performs one effect the given number of times.
        /// </summary>
        public static RepeatEffectBuilder<TEvent, TContext> Times<TEvent, TContext>(
            this IEffectBuilder<TEvent, TContext> baseEffectBuilder,
            ICalculation<TEvent, TContext> times
        )
            where TContext : class
            where TEvent : Event
        {
            return new RepeatEffectBuilder<TEvent, TContext>(baseEffectBuilder, times);
        }
    }
}
