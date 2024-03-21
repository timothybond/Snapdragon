using System.Collections.Immutable;

namespace Snapdragon
{
    // TODO: See if I can come up with a better name for this type
    /// <summary>
    /// Represents a specific card in the context of a <see cref="Game"/>, but potentially still in a
    /// <see cref="Player"/>'s <see cref="Player.Hand"/> or <see cref="Player.Library"/>.
    /// </summary>
    public record CardInstance(
        long Id,
        CardDefinition Definition,
        string Name,
        int Cost,
        int Power,
        CardState State,
        Side Side,
        Column? Column,
        int? PowerAdjustment,
        IRevealAbility<Card>? OnReveal = null,
        IOngoingAbility<Card>? Ongoing = null,
        ITriggeredCardAbility? Triggered = null,
        IMoveAbility<Card>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null
    ) : ICard
    {
        public CardInstance(
            CardDefinition definition,
            Side side,
            CardState state = CardState.InLibrary
        )
            : this(
                Ids.GetNext<ICard>(),
                definition,
                definition.Name,
                definition.Cost,
                definition.Power,
                state,
                side,
                null,
                null,
                definition.OnReveal,
                definition.Ongoing,
                definition.Triggered,
                definition.MoveAbility,
                definition.Disallowed,
                definition.PlayRestriction
            ) { }

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);

        public int? TurnRevealed => null;

        public override string ToString()
        {
            return $"{Name} ({Id}) {Cost}E {AdjustedPower}P";
        }

        public Card InPlayAt(Column column)
        {
            return new Card(
                Id,
                Definition,
                Name,
                Cost,
                Power,
                State,
                Side,
                column,
                PowerAdjustment,
                OnReveal,
                Ongoing,
                Triggered,
                MoveAbility,
                Disallowed,
                PlayRestriction
            );
        }

        public CardInstance ToCardInstance() => this;
    }
}
