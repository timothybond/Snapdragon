using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.Fluent.EffectBuilders
{
    // TODO: See if I can get rid of this extra reference to CardMovedEvent
    public record CopyToLocationBuilder<TContext>(
        ISelector<ICard, TContext> CardSelector,
        ISelector<Location, TContext> LocationSelector,
        ISingleItemSelector<Player, TContext>? PlayerSelector = null
    ) : IEffectBuilder<TContext>
        where TContext : ICard
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game).ToList();
            if (cards.Count == 0)
            {
                return new NullEffect();
            }
            var locations = LocationSelector.Get(context, game).ToList();

            if (locations.Count == 0)
            {
                return new NullEffect();
            }

            var player = PlayerSelector?.GetOrDefault(context, game);

            // TODO: Do a better job getting the actual cards in their current form
            return new AndEffect(
                cards.SelectMany(card =>
                    locations.Select(l => new AddCopyToLocation(card, l.Column, player?.Side))
                )
            );
        }
    }

    // TODO: See if I can get rid of this extra reference to CardMovedEvent
    public record CopyToLocationEventBuilder<TContext>(
        ISelector<ICard, CardMovedEvent, TContext> CardSelector,
        ISelector<Location, CardMovedEvent, TContext> LocationSelector,
        ISingleItemSelector<Player, CardMovedEvent, TContext>? PlayerSelector = null
    ) : IEffectBuilder<CardMovedEvent, TContext>
        where TContext : ICard
    {
        public IEffect Build(CardMovedEvent e, TContext context, Game game)
        {
            var cards = CardSelector.Get(e, context, game).ToList();
            if (cards.Count == 0)
            {
                return new NullEffect();
            }
            var locations = LocationSelector.Get(e, context, game).ToList();

            if (locations.Count == 0)
            {
                return new NullEffect();
            }

            var player = PlayerSelector?.GetOrDefault(e, context, game);

            // TODO: Do a better job getting the actual cards in their current form
            return new AndEffect(
                cards.SelectMany(card =>
                    locations.Select(l => new AddCopyToLocation(card, l.Column, player?.Side))
                )
            );
        }
    }

    public static class CopyToLocationExtensions
    {
        public static CopyToLocationEventBuilder<TContext> CopyToLocation<TContext>(
            this ISelector<ICard, CardMovedEvent, TContext> cardSelector,
            ISelector<Location, CardMovedEvent, TContext> locationSelector,
            ISingleItemSelector<Player, CardMovedEvent, TContext>? playerSelector = null
        )
            where TContext : class, ICard
        {
            return new CopyToLocationEventBuilder<TContext>(cardSelector, locationSelector);
        }

        public static CopyToLocationBuilder<TContext> CopyToLocation<TContext>(
            this ISelector<ICard, TContext> cardSelector,
            ISelector<Location, TContext> locationSelector,
            ISingleItemSelector<Player, TContext>? playerSelector = null
        )
            where TContext : class, ICard
        {
            return new CopyToLocationBuilder<TContext>(
                cardSelector,
                locationSelector,
                playerSelector
            );
        }
    }
}
