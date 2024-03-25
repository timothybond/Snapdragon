using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent
{
    public static class OngoingBuilderExtensions
    {
        public static Ongoing<TContext> AdjustPower<TContext>(
            this IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> builder,
            ISelector<ICard, TContext> selector,
            int amount
        )
        {
            return builder.Build(new AdjustPowerFactory<TContext>(selector, amount));
        }

        public static Ongoing<TContext> AdjustLocationPower<TContext>(
            this IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> builder,
            ISelector<Location, TContext> selector,
            int amount
        )
        {
            return builder.Build(new AdjustLocationPowerFactory<TContext>(selector, amount));
        }

        public static BlockEffectBuilder<TContext> Block<TContext>(
            this IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> builder,
            params EffectType[] effectTypes
        )
            where TContext : class
        {
            return new BlockEffectBuilder<TContext>(builder, effectTypes);
        }
    }
}
