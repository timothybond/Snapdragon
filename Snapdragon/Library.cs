using System.Collections.Immutable;

namespace Snapdragon
{
    // TODO: Consider removing this in lieu of just using the immutable list
    /// <summary>
    /// The set of <see cref="ICardInstance"/>s that a <see cref="Player"/> has not drawn yet.  By convention, the <see
    /// cref="ICardInstance"/> at the 0-index is at the top of the Library.
    /// </summary>
    public record Library(ImmutableList<ICardInstance> Cards)
    {
        public int Count => Cards.Count;

        public ICardInstance this[int index] => Cards[index];

        public Library RemoveAt(int index)
        {
            return new Library(this.Cards.RemoveAt(index));
        }
    }
}
