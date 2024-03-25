using System.Collections.Immutable;

namespace Snapdragon.Fluent
{
    public record Ongoing<TContext>(ICondition<TContext>? Condition = null) { }

    public record OngoingAdjustPower<TContext>(
        ISelector<ICard, TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingAdjustLocationPower<TContext>(
        ISelector<Location, TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingBlockLocationEffect<TContext>(
        ISelector<Location, TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ISelector<Player, TContext> PlayerSelector,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingBlockCardEffect<TContext>(
        ISelector<ICard, TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }
}
