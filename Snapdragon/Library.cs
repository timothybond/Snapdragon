using System.Collections.Immutable;

namespace Snapdragon
{
    // TODO: Consider removing this in lieu of just using the immutable list
    /// <summary>
    /// The set of <see cref="CardInstance"/>s that a <see cref="Player"/> has not drawn yet.  By convention, the <see
    /// cref="CardInstance"/> at the 0-index is at the top of the Library.
    /// </summary>
    public record Library(ImmutableList<CardInstance> Cards)
    {
        public int Count => Cards.Count;

        public CardInstance this[int index] => Cards[index];

        public Library RemoveAt(int index)
        {
            return new Library(this.Cards.RemoveAt(index));
        }
    }
}
