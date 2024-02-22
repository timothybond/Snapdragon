namespace Snapdragon.Postgresql.Data
{
    public class ItemCardDefinition
    {
        public Guid ItemId { get; set; }

        public required string CardDefinitionName { get; set; }

        public int CardOrder { get; set; }
    }
}
