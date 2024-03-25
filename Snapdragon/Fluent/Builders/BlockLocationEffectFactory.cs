using System.Collections.Immutable;

namespace Snapdragon.Fluent.Builders
{
    public record BlockLocationEffectFactory<TContext>(
        ISelector<Location, TContext> LocationSelector,
        ISelector<Player, TContext> PlayerSelector,
        params EffectType[] EffectTypes
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingBlockLocationEffect<TContext>(
                LocationSelector,
                EffectTypes.ToImmutableList(),
                PlayerSelector,
                condition
            );
        }
    }
}
