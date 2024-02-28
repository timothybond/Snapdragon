using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Api.Data
{
    public class Population
    {
        public Population()
        {
            FixedCards = new List<string>();
        }

        public required List<string> FixedCards { get; set; }

        public required List<string> AllCards { get; set; }

        public Guid Id { get; set; }

        public required string Name { get; set; }

        public int Generation { get; set; }

        // Note: I don't think I can write a cast operator for this because of the generic
        public static Population From<T>(Population<T> pop)
            where T : IGeneSequence<T>
        {
            return new Population
            {
                Id = pop.Id,
                Name = pop.Name,
                Generation = pop.Generation,
                FixedCards = pop.Genetics.GetFixedCards().Select(c => c.Name).ToList(),
                AllCards = pop.Genetics.AllPossibleCards.Select(c => c.Name).ToList()
            };
        }
    }
}
