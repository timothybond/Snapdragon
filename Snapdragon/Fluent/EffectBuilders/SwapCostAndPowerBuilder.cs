using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record SwapCostAndPowerBuilder<TContext>(ISelector<ICardInstance, TContext> CardSelector)
        : BaseCardEffectBuilder<TContext>(CardSelector)
    {
        protected override IEffect BuildCardEffect(ICardInstance card, TContext context, Game game)
        {
            return new SwapCostAndPower(card);
        }
    }

    public static class SwapCostAndPowerExtensions
    {
        public static SwapCostAndPowerBuilder<TContext> SwapCostAndPower<TContext>(
            this ISelector<ICardInstance, TContext> cardSelector
        )
        {
            return new SwapCostAndPowerBuilder<TContext>(cardSelector);
        }
    }
}
