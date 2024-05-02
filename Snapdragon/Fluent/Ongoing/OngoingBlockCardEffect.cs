using System.Collections.Immutable;

namespace Snapdragon.Fluent.Ongoing
{
    public record OngoingBlockCardEffect<TContext>(
        ISelector<ICardInstance, TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(OngoingAbilityType.BlockCardEffects, Condition) { }
}
