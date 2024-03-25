using Snapdragon.Effects;
using Snapdragon.Events;

namespace Snapdragon.Fluent.EffectBuilders
{
    // TODO: See if I can get rid of this extra reference to CardMovedEvent
    public record CopyToLocationBuilder<TContext>(
        ISelector<ICard, CardMovedEvent, TContext> CardSelector,
        ISelector<Location, CardMovedEvent, TContext> LocationSelector
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

            // TODO: Do a better job getting the actual cards in their current form
            return new AndEffect(
                cards.SelectMany(card =>
                    locations.Select(l => new AddCopyToLocation(card, l.Column))
                )
            );
        }
    }

    public static class CopyToLocationExtensions
    {
        public static CopyToLocationBuilder<TContext> CopyToLocation<TContext>(
            this ISelector<ICard, CardMovedEvent, TContext> cardSelector,
            ISelector<Location, CardMovedEvent, TContext> locationSelector
        )
            where TContext : class, ICard
        {
            return new CopyToLocationBuilder<TContext>(cardSelector, locationSelector);
        }
    }
}
