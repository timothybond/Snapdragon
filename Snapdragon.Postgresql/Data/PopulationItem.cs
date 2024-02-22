namespace Snapdragon.Postgresql.Data
{
    public class PopulationItem
    {
        public Guid PopulationId { get; set; }

        public Guid ItemId { get; set; }

        public int Generation { get; set; }
    }
}
