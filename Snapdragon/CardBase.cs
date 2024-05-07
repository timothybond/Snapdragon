using System.Collections.Immutable;
using Snapdragon.Fluent;
using Snapdragon.GameAccessors;

namespace Snapdragon
{
    /// <summary>
    /// A card, as defined and stored within a <see cref="GameKernel"/> but without the
    /// reference to that <see cref="GameKernel"/>.
    ///
    /// This is the part of a card that will not change as it gets moved around, played, etc.,
    /// but will be altered by any persistent changes to its power, or cost, or abilities.
    /// </summary>
    public record CardBase(
        long Id,
        CardDefinition Definition,
        ImmutableList<Modification> Modifications,
        string Name,
        int Cost,
        int Power,
        int? PowerAdjustment,
        OnReveal<ICard>? OnReveal = null,
        Ongoing<ICard>? Ongoing = null,
        ITriggeredAbility<ICardInstance>? Triggered = null,
        IMoveAbility<ICard>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null,
        int? TurnRevealed = null
    )
    {
        public CardBase(CardDefinition definition)
            : this(
                Ids.GetNextCard(),
                definition,
                [],
                definition.Name,
                definition.Cost,
                definition.Power,
                null,
                definition.OnReveal,
                definition.Ongoing,
                definition.Triggered,
                definition.MoveAbility,
                definition.Disallowed,
                definition.PlayRestriction
            ) { }

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);

        public override string ToString()
        {
            return $"{Name} ({Id}) {Cost}E {AdjustedPower}P";
        }

        public CardBase WithModification(Modification modification)
        {
            var modifications = Modifications.Add(modification);

            return this with
            {
                Power = Definition.Power + modifications.Sum(m => m.PowerChange ?? 0),
                Cost = Definition.Cost + modifications.Sum(m => m.CostChange ?? 0),
                Modifications = modifications
            };
        }
    }
}
