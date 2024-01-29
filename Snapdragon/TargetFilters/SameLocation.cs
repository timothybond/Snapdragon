namespace Snapdragon.TargetFilters
{
    public record SameLocation : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, GameState game)
        {
            return (card.Column == source.Column);
        }
    }
}
