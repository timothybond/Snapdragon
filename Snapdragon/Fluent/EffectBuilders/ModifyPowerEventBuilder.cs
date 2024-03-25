using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// Builder for an effect that permanently alters the power of a card
    /// (not to be confused with the ephemeral changes caused by "AdjustPower" effects)
    ///
    /// </summary>
    /// <param name="CardSelector">Selector to get affected cards.</param>
    /// <param name="Amount">Amount of power to add (or subtract)</param>
    public record ModifyPowerEventBuilder<TContext>(
        ISelector<ICard, TContext> CardSelector,
        int Amount
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game);

            var effects = cards.Select(card => new AddPowerToCard(card, Amount));

            return effects.Aggregate<IEffect, IEffect>(
                new NullEffect(),
                (accEffect, effects) => new AndEffect(accEffect, effects)
            );
        }
    }
}
