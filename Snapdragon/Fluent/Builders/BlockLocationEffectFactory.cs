using System.Collections.Immutable;

namespace Snapdragon.Fluent.Builders
{
    public record BlockLocationEffectFactory<TContext>(
        ILocationSelector<TContext> LocationSelector,
        ISideSelector<TContext> SideSelector,
        params EffectType[] EffectTypes
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingBlockLocationEffect<TContext>(
                LocationSelector,
                EffectTypes.ToImmutableList(),
                SideSelector,
                condition
            );
        }
    }
}
