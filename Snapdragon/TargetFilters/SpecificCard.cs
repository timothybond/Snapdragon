namespace Snapdragon.TargetFilters
{
    public record SpecificCard(ICardInstance Card) : ICardFilter
    {
        public bool Applies(ICardInstance card, Game game)
        {
            // Cards may be modified, but their Ids never are
            return Card.Id == card.Id;
        }
    }
}
