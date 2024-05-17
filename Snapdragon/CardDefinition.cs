using System.Collections.Immutable;
using Snapdragon.Fluent;

namespace Snapdragon
{
    public record CardDefinition(
        string Name,
        int Cost,
        int Power,
        OnReveal<ICard>? OnReveal = null,
        Ongoing<ICard>? Ongoing = null,
        ITriggeredAbility<ICardInstance>? Triggered = null,
        IMoveAbility<ICard>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null
    ) { }
}
