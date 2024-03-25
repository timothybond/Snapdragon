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
        ISelector<ICard, TContext> CardSelector,
        ICalculation<TContext> Amount
    ) : BaseCardEffectBuilder<TContext>(CardSelector)
        where TContext : class
    {
        public ModifyPowerBuilder(ISelector<ICard, TContext> CardSelector, int Amount)
            : this(CardSelector, new ConstantValue(Amount)) { }

        protected override IEffect BuildCardEffect(ICard card, TContext context, Game game)
        {
            return new AddPowerToCard(card, Amount.GetValue(context, game));
        }
    }

    public static class ModifyPowerExtensions
    {
        public static ModifyPowerBuilder<TContext> ModifyPower<TContext>(
            this ISelector<ICard, TContext> cardSelector,
            ICalculation<TContext> amount
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, amount);
        }

        public static ModifyPowerBuilder<TContext> ModifyPower<TContext>(
            this ISelector<ICard, TContext> cardSelector,
            int amount
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, amount);
        }

        public static ModifyPowerBuilder<TContext> DoublePower<TContext>(
            this ISingleItemSelector<ICard, TContext> cardSelector
        )
            where TContext : class
        {
            return new ModifyPowerBuilder<TContext>(cardSelector, cardSelector.Power());
        }
    }
}
