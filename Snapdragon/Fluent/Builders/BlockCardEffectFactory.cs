using System.Collections.Immutable;

namespace Snapdragon.Fluent.Builders
{
    public record BlockCardEffectFactory<TContext>(
        ICardSelector<TContext> Selector,
        params EffectType[] EffectTypes
    ) : IOngoingAbilityFactory<TContext>
    {
        public Ongoing<TContext> Build(ICondition<TContext>? condition = null)
        {
            return new OngoingBlockCardEffect<TContext>(
                Selector,
                EffectTypes.ToImmutableList(),
                condition
            );
        }
    }
}
