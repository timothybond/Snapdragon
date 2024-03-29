using System.Collections.Immutable;

namespace Snapdragon
{
    /// <summary>
    /// A <see cref="Player"/>'s Deck, as defined outside of the context of a specific game.
    /// </summary>
    public record Deck(ImmutableList<CardDefinition> Cards, Guid Id)
    {
        public Deck(ImmutableList<CardDefinition> Cards)
            : this(Cards, Guid.Empty) { }
    }
}
