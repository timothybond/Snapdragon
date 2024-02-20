namespace Snapdragon.Postgresql.Data
{
    public class Population
    {
        public Population()
        {
            Controller = string.Empty;
        }

        public Guid Id { get; set; }

        public Guid ExperimentId { get; set; }

        public string Controller { get; set; }
    }
}
