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

        public Library ToLibrary(Side side, bool shuffle = true)
        {
            var cards = Cards.Select(c => new CardInstance(c, side));
            if (shuffle)
            {
                cards = cards.OrderBy(card => Random.Next());
            }

            return new Library(cards.Cast<ICardInstance>().ToImmutableList());
        }
    }
}
