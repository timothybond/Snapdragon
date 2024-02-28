namespace Snapdragon.Api.Data
{
    public class Experiment
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTimeOffset Started { get; set; }

        public static explicit operator Experiment(GeneticAlgorithm.Experiment experiment) =>
            new Experiment
            {
                Id = experiment.Id,
                Name = experiment.Name,
                Started = experiment.Started
            };
    }
}
