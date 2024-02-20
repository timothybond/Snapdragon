namespace Snapdragon.Postgresql.Data
{
    public class Item
    {
        public Guid Id { get; set; }

        public Guid GenerationId { get; set; }

        public Guid FirstParent { get; set; }

        public Guid SecondParent { get; set; }
    }
}
