using System.Collections;
using System.Collections.Immutable;

namespace Snapdragon
{
    public struct SensorListReference : IReadOnlyList<Sensor<ICard>>
    {
        public SensorListReference(GameKernel kernel, ImmutableList<long> inner)
        {
            this.Kernel = kernel;
            this.Inner = inner;
        }

        GameKernel Kernel { get; }

        ImmutableList<long> Inner { get; }

        public readonly Sensor<ICard> this[int index] => Kernel.Sensors[Inner[index]];

        public readonly int Count => Inner.Count;

        public readonly IEnumerator<Sensor<ICard>> GetEnumerator()
        {
            var kernel = Kernel;
            return Inner.Select(id => kernel.Sensors[id]).GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
