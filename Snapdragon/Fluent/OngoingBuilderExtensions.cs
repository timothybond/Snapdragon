using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public static class OngoingBuilderExtensions
    {
        public static void AdjustPower<TContext>(
            this IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> builder,
            ICardSelector<TContext> selector,
            int amount
        )
        {
            builder.Build(new AdjustPowerFactory<TContext>(selector, amount));
        }
    }
}
