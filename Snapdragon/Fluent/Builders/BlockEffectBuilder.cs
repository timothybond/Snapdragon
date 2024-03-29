using Snapdragon.Fluent.Selectors;

namespace Snapdragon.Fluent.Builders
{
    public record BlockEffectBuilder<TContext>(
        IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> OngoingBuilder,
        params EffectType[] EffectTypes
    )
        where TContext : class
    {
        public Ongoing<TContext> ForLocation(ISelector<Location, TContext> locationSelector)
        {
            return ForLocationAndSide(locationSelector, new BothSides());
        }

        public Ongoing<TContext> ForLocationAndSide(
            ISelector<Location, TContext> locationSelector,
            ISelector<Player, TContext> playerSelector
        )
        {
            return OngoingBuilder.Build(
                new BlockLocationEffectFactory<TContext>(
                    locationSelector,
                    playerSelector,
                    EffectTypes
                )
            );
        }

        public Ongoing<TContext> ForCards(ISelector<ICardInstance, TContext> cardSelector)
        {
            return OngoingBuilder.Build(
                new BlockCardEffectFactory<TContext>(cardSelector, EffectTypes)
            );
        }
    }
}
