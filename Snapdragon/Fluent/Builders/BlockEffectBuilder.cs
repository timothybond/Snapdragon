using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent.Builders
{
    public record BlockEffectBuilder<TContext>(
        IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> OngoingBuilder,
        params EffectType[] EffectTypes
    )
        where TContext : class
    {
        public Ongoing<TContext> ForLocation(ILocationSelector<TContext> locationSelector)
        {
            return ForLocationAndSide(locationSelector, new BothSides());
        }

        public Ongoing<TContext> ForLocationAndSide(
            ILocationSelector<TContext> locationSelector,
            ISideSelector<TContext> sideSelector
        )
        {
            return OngoingBuilder.Build(
                new BlockLocationEffectFactory<TContext>(
                    locationSelector,
                    sideSelector,
                    EffectTypes
                )
            );
        }

        public Ongoing<TContext> ForCards(ICardSelector<TContext> cardSelector)
        {
            return OngoingBuilder.Build(
                new BlockCardEffectFactory<TContext>(cardSelector, EffectTypes)
            );
        }
    }
}
