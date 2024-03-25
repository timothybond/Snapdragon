using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public abstract record BaseCardEffectBuilder<TContext>(ISelector<ICard, TContext> CardSelector)
        : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game).ToList();

            if (cards.Count == 0)
            {
                return new NullEffect();
            }

            // Technically this does the same thing as the following,
            // but creates instantiates fewer objects.
            if (cards.Count == 1)
            {
                return this.BuildCardEffect(cards[0], context, game);
            }

            var effects = cards.Select(card => this.BuildCardEffect(card, context, game));
            return new AndEffect(effects);
        }

        protected abstract IEffect BuildCardEffect(ICard card, TContext context, Game game);
    }

    public abstract record BaseCardEffectBuilder<TEvent, TContext>(
        ISelector<ICard, TEvent, TContext> CardSelector
    ) : IEffectBuilder<TEvent, TContext>
        where TEvent : Event
    {
        public IEffect Build(TEvent e, TContext context, Game game)
        {
            var cards = CardSelector.Get(e, context, game).ToList();

            if (cards.Count == 0)
            {
                return new NullEffect();
            }

            // Technically this does the same thing as the following,
            // but creates instantiates fewer objects.
            if (cards.Count == 1)
            {
                return this.BuildCardEffect(cards[0], e, context, game);
            }

            var effects = cards.Select(card => this.BuildCardEffect(card, e, context, game));
            return new AndEffect(effects);
        }

        protected abstract IEffect BuildCardEffect(
            ICard card,
            TEvent e,
            TContext context,
            Game game
        );
    }
}
