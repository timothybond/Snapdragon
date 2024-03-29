using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record MoveToHereBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
        where TContext : IObjectWithPossibleColumn
    {
        protected override IEffect BuildCardEffect(ICardInstance card, TContext context, Game game)
        {
            if (context.Column == null)
            {
                return new NullEffect();
            }

            if (card.Column == null)
            {
                // TODO: Consider whether this is a good default
                return new NullEffect();
            }

            return new MoveCard(card, card.Column.Value, context.Column.Value, true);
        }
    }

    public static class MoveToHereExtensions
    {
        public static MoveToHereBuilder<TContext> MoveToHere<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
            where TContext : IObjectWithPossibleColumn
        {
            return new MoveToHereBuilder<TContext>(cardSelector);
        }
    }
}
