using System.Collections.Immutable;
using Snapdragon.Fluent;

namespace Snapdragon
{
    public record CardDefinition(
        string Name,
        int Cost,
        int Power,
        OnReveal<Card>? OnReveal = null,
        Ongoing<Card>? Ongoing = null,
        ITriggeredAbility<ICard>? Triggered = null,
        IMoveAbility<Card>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null
    ) { }
}
