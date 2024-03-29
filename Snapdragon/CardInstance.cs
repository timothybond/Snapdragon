﻿using Snapdragon.Fluent;
using System.Collections.Immutable;

namespace Snapdragon
{
    /// <summary>
    /// The combination of the core attributes of a <see cref="CardBase"/> and a reference to a <see cref="GameKernel"/>
    /// that allows us to retrieve the ephemeral attributes from that <see cref="GameKernel"/>.
    ///
    /// Note that because both <see cref="CardBase"/> and <see cref="GameKernel"/> can only be altered
    /// by creating new instances, the values of this object are unreliable whenever anything changes.
    ///
    /// However, it is by design fairly low effort to create a new one of these after any change.
    /// </summary>
    public record CardInstance(CardBase Base, GameKernel Kernel) : ICardInstance
    {
        public long Id => Base.Id;
        public CardDefinition Definition => Base.Definition;
        public string Name => Base.Name;
        public int Cost => Base.Cost;
        public int Power => Base.Power;
        public int? PowerAdjustment => Base.PowerAdjustment;
        public OnReveal<ICard>? OnReveal => Base.OnReveal;
        public Ongoing<ICard>? Ongoing => Base.Ongoing;
        public ITriggeredAbility<ICardInstance>? Triggered => Base.Triggered;
        public IMoveAbility<ICard>? MoveAbility => Base.MoveAbility;
        public ImmutableList<EffectType>? Disallowed => Base.Disallowed;
        public IPlayRestriction? PlayRestriction => Base.PlayRestriction;
        public int? TurnRevealed => Base.TurnRevealed;

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);

        public CardState State => Kernel.CardStates[this.Id];
        public Side Side => Kernel.CardSides[this.Id];
        public Column? Column => Kernel.CardLocations[this.Id];

        public override string ToString()
        {
            return $"{Name} ({Id}) {Cost}E {AdjustedPower}P";
        }
    }
}
