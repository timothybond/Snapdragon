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

        // TODO: Replace with a cast operator now that I've gotten rid of the generics
        public static Population From(GeneticAlgorithm.Population pop)
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
