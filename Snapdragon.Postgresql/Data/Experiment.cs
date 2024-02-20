namespace Snapdragon.Postgresql.Data
{
    public class Experiment
    {
        public Experiment()
        {
            Name = string.Empty;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public static explicit operator GeneticAlgorithm.Experiment(Experiment e) =>
            new GeneticAlgorithm.Experiment(e.Id, e.Name, e.Created);

        public static explicit operator Experiment(GeneticAlgorithm.Experiment e) =>
            new Experiment
            {
                Id = e.Id,
                Name = e.Name,
                Created = e.Started
            };
    }
}
