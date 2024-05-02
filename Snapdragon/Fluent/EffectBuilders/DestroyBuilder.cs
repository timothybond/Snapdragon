using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    /// <summary>
    /// Builder for an effect that destroys cards (in play).
    /// </summary>
    /// <param name="CardSelector">Selector to get affected cards.</param>
    /// <param name="Amount">Amount of power to add (or subtract)</param>
    public record DestroyBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game);

            var effects = cards.Select(card => new DestroyCardInPlay(card));

            return new AndEffect(effects);
        }
    }

    /// <summary>
    /// Builder for an effect that destroys cards (in play).
    /// </summary>
    /// <param name="CardSelector">Selector to get affected cards.</param>
    /// <param name="Amount">Amount of power to add (or subtract)</param>
    public record DestroyBuilder<TEvent, TContext>(
        ISelector<ICardInstance, TEvent, TContext> CardSelector
    ) : IEffectBuilder<TEvent, TContext>
        where TEvent : Event
    {
        public IEffect Build(TEvent e, TContext context, Game game)
        {
            var cards = CardSelector.Get(e, context, game);

            var effects = cards.Select(card => new DestroyCardInPlay(card));

            return new AndEffect(effects);
        }
    }

    public static class DestroyExtensions
    {
        public static DestroyBuilder<TContext> Destroy<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
            where TContext : class
        {
            return new DestroyBuilder<TContext>(cardSelector);
        }

        public static DestroyBuilder<TEvent, TContext> Destroy<TEvent, TContext>(
            this ISelector<ICardInstance, TEvent, TContext> cardSelector
        )
            where TContext : class
            where TEvent : Event
        {
            return new DestroyBuilder<TEvent, TContext>(cardSelector);
        }
    }
}
