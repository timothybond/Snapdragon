﻿using System.Collections.Immutable;

namespace Snapdragon
{
    public record CardDefinition(
        string Name,
        int Cost,
        int Power,
        IRevealAbility<Card>? OnReveal = null,
        IOngoingAbility<Card>? Ongoing = null,
        ITriggeredCardAbility? Triggered = null,
        IMoveAbility<Card>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null
    ) { }
}
