namespace Snapdragon.TargetFilters
{
    public record SpecificCard(Card Card) : ICardFilter
    {
        public bool Applies(Card card, Game game)
        {
            // Cards may be modified, but their Ids never are
            return Card.Id == card.Id;
        }
    }
}
