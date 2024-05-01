using System.Collections.Immutable;

namespace Snapdragon.Fluent.Ongoing
{
    public record OngoingBlockLocationEffect<TContext>(
        ISelector<Location, TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ISelector<Player, TContext> PlayerSelector,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition)
    { }
}
