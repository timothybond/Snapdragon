using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record MoveLeftBuilder<TContext>(ISelector<ICard, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICard card, TContext context, Game game)
        {
            // TODO: Throw an error if we select non-in-play cards?
            if (card is Card actualCard)
            {
                return new MoveCardLeft(actualCard);
            }
            else
            {
                return new NullEffect();
            }
        }
    }

    public record MoveLeftBuilder<TEvent, TContext>(ISelector<ICard, TEvent, TContext> CardSelector)
        : BaseCardEffectBuilder<TEvent, TContext>(CardSelector)
        where TEvent : Event
    {
        protected override IEffect BuildCardEffect(
            ICard card,
            TEvent e,
            TContext context,
            Game game
        )
        {
            // TODO: Throw an error if we select non-in-play cards?
            if (card is Card actualCard)
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
            this ISelector<ICard, TContext> cardSelector
        )
        {
            return new MoveLeftBuilder<TContext>(cardSelector);
        }

        public static MoveLeftBuilder<TEvent, TContext> MoveLeft<TEvent, TContext>(
            this ISelector<ICard, TEvent, TContext> cardSelector
        )
            where TEvent : Event
        {
            return new MoveLeftBuilder<TEvent, TContext>(cardSelector);
        }
    }
}
