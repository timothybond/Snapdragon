namespace Snapdragon.Api.Data
{
    public class CardCount
    {
        public required string Name { get; set; }

        public required IReadOnlyList<int> Counts { get; set; }
    }
}
