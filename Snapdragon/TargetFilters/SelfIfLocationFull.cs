namespace Snapdragon.TargetFilters
{
    public record SelfIfLocationFull : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, GameState game)
        {
            // TODO: Handle locations that restrict the number of cards played

            var column = card.Column;

            if (column == null)
            {
                return false;
            }

            // Note that we can't always rely on the Card instances to stay the same,
            // because any adjustment creates a new instance with slightly different attributes.
            return (card.Id == source.Id) && (game[column.Value][card.Side].Count == 4);
        }
    }
}
