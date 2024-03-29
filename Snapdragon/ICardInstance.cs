using Snapdragon.Fluent;
using System.Collections.Immutable;

namespace Snapdragon
{
    public interface ICardInstance : IObjectWithSide, IRevealableObject
    {
        long Id { get; }
        CardBase Base { get; }
        CardDefinition Definition { get; }
        string Name { get; }
        int Cost { get; }
        int Power { get; }
        CardState State { get; }
        int? PowerAdjustment { get; }
        int AdjustedPower { get; }
        OnReveal<ICard>? OnReveal { get; }
        Ongoing<ICard>? Ongoing { get; }
        ITriggeredAbility<ICardInstance>? Triggered { get; }
        IMoveAbility<ICard>? MoveAbility { get; }
        ImmutableList<EffectType>? Disallowed { get; }
        IPlayRestriction? PlayRestriction { get; }
    }
}
