namespace Snapdragon.GeneticAlgorithm
{
    public record Population<T>(
        Genetics<T> Genetics,
        IReadOnlyList<T> Items,
        string Name,
        Guid Id,
        Guid ExperimentId,
        int Generation,
        DateTimeOffset Created,
        IReadOnlyList<int>? Wins = null
    )
        where T : IGeneSequence<T>
    {
        public Population(Genetics<T> genetics, int size, string name, Guid experimentId)
            : this(
                genetics,
                genetics.GetRandomPopulation(size),
                name,
                Guid.NewGuid(),
                experimentId,
                0,
                DateTimeOffset.UtcNow
            ) { }

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
                Generation = Generation + 1,
                Created = DateTimeOffset.UtcNow,
                Wins = null
            };
        }
    }
}
