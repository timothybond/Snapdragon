using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record CopyToHandBuilder<TContext>(
        ISingleCardSelector<TContext> CardSelector,
        ICardTransform? Transform = null
    ) : IEffectBuilder<TContext>
    {
        public IEffect Build(TContext context, Game game)
        {
            var card = CardSelector.GetOrDefault(context, game);

            if (card == null)
            {
                return new NullEffect();
            }

            return new AddCopyToHand(card, Transform);
        }
    }

    public static class CopyToHandExtensions
    {
        public static CopyToHandBuilder<TContext> CopyToHand<TContext>(
            this ISingleCardSelector<TContext> cardSelector,
            ICardTransform? transform
        )
            where TContext : class
        {
            return new CopyToHandBuilder<TContext>(cardSelector, transform);
        }
    }
}
