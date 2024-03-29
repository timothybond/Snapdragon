using System.Collections;
using System.Collections.Immutable;

namespace Snapdragon
{
    public struct CardListReference : IReadOnlyList<ICardInstance>
    {
        public CardListReference(GameKernel kernel, ImmutableList<long> inner)
        {
            this.Kernel = kernel;
            this.Inner = inner;
        }

        GameKernel Kernel { get; }

        ImmutableList<long> Inner { get; }

        public readonly ICardInstance this[int index] => Kernel[Inner[index]];

        public readonly int Count => Inner.Count;

        public readonly IEnumerator<ICardInstance> GetEnumerator()
        {
            var kernel = Kernel;
            return Inner.Select(id => kernel[id]).GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
