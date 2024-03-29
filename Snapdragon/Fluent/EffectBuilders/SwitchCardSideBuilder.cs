using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record SwitchCardSideBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICardInstance card, TContext context, Game game)
        {
            if (card is ICard inPlayCard)
            {
                return new SwitchCardSide(inPlayCard);
            }
            else
            {
                // TODO: Throw an error? Not sure what would produce this.
                return new NullEffect();
            }
        }
    }

    public static class SwitchSidesExtensions
    {
        public static SwitchCardSideBuilder<TContext> SwitchSides<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
            where TContext : class
        {
            return new SwitchCardSideBuilder<TContext>(cardSelector);
        }
    }
}
