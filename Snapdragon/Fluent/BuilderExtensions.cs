using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.EffectBuilders;

namespace Snapdragon.Fluent
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Creates an ability that will permanently modify the Power of the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to modify the Power of.</param>
        /// <param name="amount">Amount to increase (or decrease) the power of the cards by.</param>
        /// <returns>The constructed ability that will have the power-modification effect.</returns>
        public static TAbility ModifyPower<TAbility, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TContext>> builder,
            ICardSelector<TContext> cardSelector,
            int amount
        )
            where TContext : class
        {
            return builder.Build(new ModifyPowerBuilder<TContext>(cardSelector, amount));
        }

        /// <summary>
        /// Creates an ability that will permanently modify the Power of the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to modify the Power of.</param>
        /// <param name="amount">Calculation for the amount to increase (or decrease) the power of the cards by.</param>
        /// <returns>The constructed ability that will have the power-modification effect.</returns>
        public static TAbility ModifyPower<TAbility, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TContext>> builder,
            ICardSelector<TContext> cardSelector,
            ICalculation<TContext> amount
        )
            where TContext : class
        {
            return builder.Build(new ModifyPowerBuilder<TContext>(cardSelector, amount));
        }

        /// <summary>
        /// Creates an ability that will permanently modify the Power of the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to modify the Power of.</param>
        /// <param name="amount">Amount to increase (or decrease) the power of the cards by.</param>
        /// <returns>The constructed ability that will have the power-modification effect.</returns>
        public static TAbility ModifyPower<TAbility, TEvent, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TEvent, TContext>> builder,
            ICardSelector<TContext> cardSelector,
            int amount
        )
            where TEvent : Event
            where TContext : class
        {
            return builder.Build(new ModifyPowerBuilder<TContext>(cardSelector, amount));
        }

        /// <summary>
        /// Creates an ability that will permanently modify the Power of the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to modify the Power of.</param>
        /// <param name="amount">Amount to increase (or decrease) the power of the cards by.</param>
        /// <returns>The constructed ability that will have the power-modification effect.</returns>
        public static TAbility ModifyPower<TAbility, TEvent, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TEvent, TContext>> builder,
            ICardSelector<TContext> cardSelector,
            ICalculation<TContext> amount
        )
            where TEvent : Event
            where TContext : class
        {
            return builder.Build(new ModifyPowerBuilder<TContext>(cardSelector, amount));
        }

        /// <summary>
        /// Creates an ability that will permanently double the Power of a single card.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for card to double the Power of. Must return no more than 1 card always.</param>
        /// <param name="amount">Amount to increase (or decrease) the power of the cards by.</param>
        /// <returns>The constructed ability that will have the power-modification effect.</returns>
        public static TAbility DoublePower<TAbility, TEvent, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TEvent, TContext>> builder,
            ISingleCardSelector<TContext> cardSelector
        )
            where TEvent : Event
            where TContext : class
        {
            return builder.Build(
                new ModifyPowerBuilder<TContext>(cardSelector, cardSelector.Power())
            );
        }

        /// <summary>
        /// Creates an ability that will trigger discarding the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to discard.</param>
        /// <returns>The constructed ability that will cause the discards.</returns>
        public static TAbility Discard<TAbility, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TContext>> builder,
            ICardSelector<TContext> cardSelector
        )
        {
            return builder.Build(new DiscardBuilder<TContext>(cardSelector));
        }

        /// <summary>
        /// Creates an ability that will trigger destroying the selected cards.
        /// </summary>
        /// <param name="builder">A builder for the ability.</param>
        /// <param name="cardSelector">Selector for cards to destroy.</param>
        /// <returns>The constructed ability that will cause the destruction.</returns>
        public static TAbility Destroy<TAbility, TContext>(
            this IBuilder<TAbility, TContext, IEffectBuilder<TContext>> builder,
            ICardSelector<TContext> cardSelector
        )
        {
            return builder.Build(new DiscardBuilder<TContext>(cardSelector));
        }
    }
}
