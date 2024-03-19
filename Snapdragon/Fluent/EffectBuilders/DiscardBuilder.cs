using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// Builder for an effect that discards cards from player hands.
    /// </summary>
    /// <param name="CardSelector">Selector to get affected cards.</param>
    /// <param name="Amount">Amount of power to add (or subtract)</param>
    public record DiscardBuilder<TContext>(ICardSelector<TContext> CardSelector)
        : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game);

            var effects = cards.Select(card => new DiscardCard(card));

            return effects.Aggregate<IEffect, IEffect>(
                new NullEffect(),
                (accEffect, effects) => new AndEffect(accEffect, effects)
            );
        }
    }
}
