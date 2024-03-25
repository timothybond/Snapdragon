using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record CopyToHandBuilder<TEvent, TContext>(
        ISingleItemSelector<ICard, TEvent, TContext> CardSelector,
        ICardTransform? Transform = null,
        ISelector<Player, TEvent, TContext>? PlayerSelector = null
    ) : IEffectBuilder<TEvent, TContext>
        where TEvent : Event
    {
        public IEffect Build(TEvent e, TContext context, Game game)
        {
            var card = CardSelector.GetOrDefault(e, context, game);

            if (card == null)
            {
                return new NullEffect();
            }

            var players =
                PlayerSelector?.Get(e, context, game).ToList()
                ?? new List<Player> { game[card.Side] };

            if (players.Count == 0)
            {
                return new NullEffect();
            }

            if (players.Count == 1)
            {
                return new AddCopyToHand(card, Transform, players[0]);
            }

            return new AndEffect(players.Select(side => new AddCopyToHand(card, Transform, side)));
        }
    }

    public static class CopyToHandExtensions
    {
        public static CopyToHandBuilder<Event, TContext> CopyToHand<TContext>(
            this ISingleItemSelector<ICard, TContext> cardSelector,
            ICardTransform? transform = null
        )
            where TContext : class
        {
            return new CopyToHandBuilder<Event, TContext>(cardSelector, transform);
        }

        public static CopyToHandBuilder<Event, TContext> CopyToHand<TContext>(
            this ISingleItemSelector<ICard, TContext> cardSelector,
            ISelector<Player, TContext> playerSelector,
            ICardTransform? transform = null
        )
            where TContext : class
        {
            return new CopyToHandBuilder<Event, TContext>(cardSelector, transform, playerSelector);
        }

        public static CopyToHandBuilder<TEvent, TContext> CopyToHand<TEvent, TContext>(
            this ISingleItemSelector<ICard, TEvent, TContext> cardSelector,
            ICardTransform? transform = null
        )
            where TContext : class
            where TEvent : Event
        {
            return new CopyToHandBuilder<TEvent, TContext>(cardSelector, transform);
        }

        public static CopyToHandBuilder<TEvent, TContext> CopyToHand<TEvent, TContext>(
            this ISingleItemSelector<ICard, TEvent, TContext> cardSelector,
            ISelector<Player, TEvent, TContext> playerSelector,
            ICardTransform? transform = null
        )
            where TContext : class
            where TEvent : Event
        {
            return new CopyToHandBuilder<TEvent, TContext>(cardSelector, transform, playerSelector);
        }
    }
}
