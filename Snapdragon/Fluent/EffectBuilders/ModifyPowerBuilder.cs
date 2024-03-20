using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// Builder for an effect that permanently alters the power of a card
    /// (not to be confused with the ephemeral changes caused by "AdjustPower" effects).
    /// </summary>
    /// <param name="CardSelector">Selector to get affected cards.</param>
    /// <param name="Amount">Amount of power to add (or subtract)</param>
    public record ModifyPowerBuilder<TContext>(
        ICardSelector<TContext> CardSelector,
        ICalculation<TContext> Amount
    ) : IEffectBuilder<TContext>
        where TContext : class
    {
        public ModifyPowerBuilder(ICardSelector<TContext> CardSelector, int Amount)
            : this(CardSelector, new ConstantValue(Amount)) { }

        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game).ToList();

            if (cards.Count == 0)
            {
                return new NullEffect();
            }

            var effects = cards.Select(card => new AddPowerToCard(
                card,
                Amount.GetValue(context, game)
            ));

            return effects.Aggregate<IEffect, IEffect>(
                new NullEffect(),
                (accEffect, effects) => new AndEffect(accEffect, effects)
            );
        }
    }

    public static class ModifyPowerExtensions
    {
        public static ModifyPowerBuilder<TContext> ModifyPower<TContext>(
            this ICardSelector<TContext> cardSelector,
            ICalculation<TContext> amount
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, amount);
        }

        public static ModifyPowerBuilder<TContext> ModifyPower<TContext>(
            this ICardSelector<TContext> cardSelector,
            int amount
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, amount);
        }

        public static ModifyPowerBuilder<TContext> DoublePower<TContext>(
            this ISingleCardSelector<TContext> cardSelector
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, cardSelector.Power());
        }
    }
}
