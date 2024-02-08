using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Runner
{
    public record Population<T>(
        Genetics<T> Genetics,
        IReadOnlyList<T> Items,
        string Name,
        IReadOnlyList<int>? Wins = null
    )
        where T : IGeneSequence<T>
    {
        public Population(Genetics<T> genetics, int size, string name)
            : this(genetics, genetics.GetRandomPopulation(size), name) { }

        public Population<T> Reproduce(int pinTopX = 4)
        {
            if (this.Wins == null)
            {
                throw new InvalidOperationException(
                    "'Wins' was unset when reproduction was attempted."
                );
            }

            var newItems = Genetics.ReproducePopulation(Items, Wins, pinTopX);

            return this with
            {
                Items = newItems,
                Wins = null
            };
        }
    }
}
