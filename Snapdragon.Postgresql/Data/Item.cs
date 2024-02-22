namespace Snapdragon.Postgresql.Data
{
    public class Item
    {
        public Guid Id { get; set; }

        public Guid? FirstParentId { get; set; }

        public Guid? SecondParentId { get; set; }

        public required string Controller { get; set; }
    }
}
