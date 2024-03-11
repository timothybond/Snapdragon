namespace Snapdragon.Postgresql.Data
{
    internal class CardCountRow
    {
        public required string Name { get; set; }

        public int Generation { get; set; }

        public int Count { get; set; }
    }
}
