namespace Snapdragon.Postgresql.Data
{
    public class PopulationCardDefinition
    {
        public Guid PopulationId { get; set; }

        public required string CardDefinitionName { get; set; }

        public bool Fixed { get; set; }
    }
}
