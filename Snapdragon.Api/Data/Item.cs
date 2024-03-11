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

        // TODO: Replace with cast operator
        public static Item From(GeneSequence item)
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
