﻿using System.Collections.Immutable;
using Snapdragon.Fluent;

namespace Snapdragon
{
    public interface ICard : IObjectWithSide, IRevealableObject
    {
        long Id { get; }
        CardDefinition Definition { get; }
        string Name { get; }
        int Cost { get; }
        int Power { get; }
        CardState State { get; }
        int? PowerAdjustment { get; }
        OnReveal<Card>? OnReveal { get; }
        Ongoing<Card>? Ongoing { get; }
        ITriggeredAbility<ICard>? Triggered { get; }
        IMoveAbility<Card>? MoveAbility { get; }
        ImmutableList<EffectType>? Disallowed { get; }
        IPlayRestriction? PlayRestriction { get; }

        /// <summary>
        /// Creates a <see cref="Card"/> from this item. Note that this will not necessarily set the <see cref="ICard.State"/> to
        /// <see cref="CardState.InPlay"/>, because technically <see cref="CardState.PlayedButNotRevealed"/> is also valid.
        /// </summary>
        Card InPlayAt(Column column);

        /// <summary>
        /// Gets the item as a <see cref="CardInstance"/>, regardless of what type it currently is.
        /// </summary>
        /// <returns></returns>
        CardInstance ToCardInstance();
    }
}
