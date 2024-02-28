using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Api.Data
{
    public class Item
    {
        public Item()
        {
            Cards = new List<string>();
        }

        public Guid Id { get; set; }
        public Guid? FirstParentId { get; set; }
        public Guid? SecondParentId { get; set; }
        public required List<string> Cards { get; set; }

        // Note: I don't think I can write a cast operator for this because of the generic
        public static Item From<T>(IGeneSequence<T> item)
            where T : IGeneSequence<T>
        {
            return new Item
            {
                Id = item.Id,
                FirstParentId = item.FirstParentId,
                SecondParentId = item.SecondParentId,
                Cards = item.GetCards().Select(cd => cd.Name).OrderBy(n => n).ToList()
            };
        }
    }
}
