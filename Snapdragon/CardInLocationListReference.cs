using System.Collections;
using System.Collections.Immutable;

namespace Snapdragon
{
    public struct CardInLocationListReference : IReadOnlyList<ICard>
    {
        public CardInLocationListReference(GameKernel kernel, ImmutableList<long> inner)
        {
            this.Kernel = kernel;
            this.Inner = inner;
        }

        GameKernel Kernel { get; }

        ImmutableList<long> Inner { get; }

        public readonly ICard this[int index] => Kernel[Inner[index]] as ICard;

        public readonly int Count => Inner.Count;

        public readonly IEnumerator<ICard> GetEnumerator()
        {
            var kernel = Kernel;
            return Inner.Select(id => kernel[id]).Cast<ICard>().GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
