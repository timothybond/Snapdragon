using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record SwapCostAndPowerBuilder<TContext>(ISelector<ICard, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICard card, TContext context, Game game)
        {
            return new SwapCostAndPower(card);
        }
    }

    public static class SwapCostAndPowerExtensions
    {
        public static SwapCostAndPowerBuilder<TContext> SwapCostAndPower<TContext>(
            this ISelector<ICard, TContext> cardSelector
        )
        {
            return new SwapCostAndPowerBuilder<TContext>(cardSelector);
        }
    }
}
