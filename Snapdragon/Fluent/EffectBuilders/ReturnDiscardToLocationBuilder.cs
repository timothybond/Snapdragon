using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record ReturnDiscardToLocationBuilder<TContext>(
        ISelector<ICardInstance, TContext> CardSelector,
        ISingleItemSelector<Location, TContext> LocationSelector
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var cards = CardSelector.Get(context, game).ToList();

            if (cards.Count == 0)
            {
                return new NullEffect();
            }

            var location = LocationSelector.GetOrDefault(context, game);

            if (location == null)
            {
                return new NullEffect();
            }

            return new AndEffect(
                cards.Select(card => new ReturnDiscardToLocation(card, location.Column))
            );
        }
    }

    public static class ReturnDiscardToLocationExtensions
    {
        public static ReturnDiscardToLocationBuilder<TContext> ReturnDiscardTo<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector,
            ISingleItemSelector<Location, TContext> locationSelector
        )
        {
            return new ReturnDiscardToLocationBuilder<TContext>(cardSelector, locationSelector);
        }
    }
}
