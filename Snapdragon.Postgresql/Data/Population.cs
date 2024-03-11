namespace Snapdragon.Postgresql.Data
{
    public class Population
    {
        public Guid Id { get; set; }

        public Guid ExperimentId { get; set; }

        public required string Name { get; set; }

        public required string Controller { get; set; }

        public int MutationPer { get; set; }

        public required string OrderBy { get; set; }

        public static Population FromPopulation(GeneticAlgorithm.Population population)
        {
            return new Population
            {
                Id = population.Id,
                Name = population.Name,
                ExperimentId = population.ExperimentId,
                Controller = population.Genetics.GetControllerString() ?? string.Empty,
                MutationPer = population.Genetics.MutationPer,
                OrderBy = population.Genetics.OrderBy.ToString() ?? string.Empty
            };
        }
    }
}
