using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record SwitchCardSideBuilder<TContext>(ISelector<ICard, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICard card, TContext context, Game game)
        {
            if (card is Card inPlayCard)
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
            this ISelector<ICard, TContext> cardSelector
        )
            where TContext : class
        {
            return new SwitchCardSideBuilder<TContext>(cardSelector);
        }
    }
}
