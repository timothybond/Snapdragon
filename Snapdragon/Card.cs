﻿using Snapdragon.Fluent;
using System.Collections.Immutable;

namespace Snapdragon
{
    /// <summary>
    /// A card that has actually been played into a particular <see cref="Column"/>.
    ///
    /// This is the "main" class for items that have meaningful effects in a <see cref="Game"/>.
    ///
    /// In comparison, the <see cref="CardInstance"/> class exists to track cards that are out of play
    /// (in hands, in decks, discarded, or destroyed), but mostly just for scenarios where they might
    /// return to play, which is why even on that type, abilities are mostly referred to using the
    /// type <see cref="Card"/>.
    /// </summary>
    public record Card(
        long Id,
        CardDefinition Definition,
        string Name,
        int Cost,
        int Power,
        CardState State,
        Side Side,
        Column Column,
        int? PowerAdjustment,
        ImmutableList<Modification> Modifications,
        OnReveal<ICard>? OnReveal = null,
        Ongoing<ICard>? Ongoing = null,
        ITriggeredAbility<ICardInstance>? Triggered = null,
        IMoveAbility<ICard>? MoveAbility = null,
        ImmutableList<EffectType>? Disallowed = null,
        IPlayRestriction? PlayRestriction = null,
        int? TurnRevealed = null
    ) : ICard
    {
        public Card(CardDefinition definition, Side side, Column column)
            : this(
                Ids.GetNextCard(),
                definition,
                definition.Name,
                definition.Cost,
                definition.Power,
                CardState.InPlay,
                side,
                column,
                null,
                [],
                definition.OnReveal,
                definition.Ongoing,
                definition.Triggered,
                definition.MoveAbility,
                definition.Disallowed,
                definition.PlayRestriction
            )
        { }

        public int AdjustedPower => this.Power + (this.PowerAdjustment ?? 0);

        Column? IObjectWithPossibleColumn.Column => this.Column;

        public override string ToString()
        {
            return $"{Name} ({Id}) {Cost}E {AdjustedPower}P";
        }

        public CardInstance ToCardInstance()
        {
            return new CardInstance(
                Id,
                Definition,
                Name,
                Cost,
                Power,
                State,
                Side,
                Column,
                PowerAdjustment,
                Modifications,
                OnReveal,
                Ongoing,
                Triggered,
                MoveAbility,
                Disallowed,
                PlayRestriction
            );
        }

        public Card InPlayAt(Column column)
        {
            return this with { Column = column };
        }

        public ICard WithModification(Modification modification)
        {
            // TODO: Determine if there are cases where we need to avoid recalculating
            return this with
            {
                Modifications = this.Modifications.Add(modification),
                Power = this.Power + (modification.PowerChange ?? 0),
                Cost = this.Cost + (modification.CostChange ?? 0)
            };
        }

        ICardInstance ICardInstance.WithModification(Modification modification)
        {
            return this.WithModification(modification);
        }
    }
}
