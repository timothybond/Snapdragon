using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record ModifyPowerBuilder<TContext>(ICardSelector<TContext> CardSelector, int Amount)
        : IEffectBuilder<TContext>
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
