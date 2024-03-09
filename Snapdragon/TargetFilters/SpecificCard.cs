namespace Snapdragon.TargetFilters
{
    public record SpecificCard(ICard Card) : ICardFilter
    {
        public bool Applies(ICard card, Game game)
        {
            // Cards may be modified, but their Ids never are
            return Card.Id == card.Id;
        }
    }
}
