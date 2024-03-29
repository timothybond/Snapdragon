using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record MoveLeftBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICardInstance card, TContext context, Game game)
        {
            // TODO: Throw an error if we select non-in-play cards?
            if (card is ICard actualCard)
            {
                return new MoveCardLeft(actualCard);
            }
            else
            {
                return new NullEffect();
            }
        }
    }

    public record MoveLeftBuilder<TEvent, TContext>(ISelector<ICardInstance, TEvent, TContext> CardSelector)
        : BaseCardEffectBuilder<TEvent, TContext>(CardSelector)
        where TEvent : Event
    {
        protected override IEffect BuildCardEffect(
            ICardInstance card,
            TEvent e,
            TContext context,
            Game game
        )
        {
            // TODO: Throw an error if we select non-in-play cards?
            if (card is ICard actualCard)
            {
                return new MoveCardLeft(actualCard);
            }
            else
            {
                return new NullEffect();
            }
        }
    }

    public static class MoveCardLeftExtensions
    {
        public static MoveLeftBuilder<TContext> MoveLeft<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
        {
            return new MoveLeftBuilder<TContext>(cardSelector);
        }

        public static MoveLeftBuilder<TEvent, TContext> MoveLeft<TEvent, TContext>(
            this ISelector<ICardInstance, TEvent, TContext> cardSelector
        )
            where TEvent : Event
        {
            return new MoveLeftBuilder<TEvent, TContext>(cardSelector);
        }
    }
}
