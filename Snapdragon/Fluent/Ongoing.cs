using System.Collections.Immutable;

namespace Snapdragon.Fluent
{
    public record Ongoing<TContext>(ICondition<TContext>? Condition = null) { }

    public record OngoingAdjustPower<TContext>(
        ICardSelector<TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingAdjustLocationPower<TContext>(
        ILocationSelector<TContext> Selector,
        int Amount,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingBlockLocationEffect<TContext>(
        ILocationSelector<TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ISideSelector<TContext> Side,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }

    public record OngoingBlockCardEffect<TContext>(
        ICardSelector<TContext> Selector,
        ImmutableList<EffectType> BlockedEffects,
        ICondition<TContext>? Condition = null
    ) : Ongoing<TContext>(Condition) { }
}
