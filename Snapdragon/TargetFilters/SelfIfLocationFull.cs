namespace Snapdragon.TargetFilters
{
    public record SelfIfLocationFull : ICardFilter<ICard>
    {
        public bool Applies(ICardInstance card, ICard source, Game game)
        {
            // TODO: Handle locations that restrict the number of cards played

            var column = card.Column;

            if (column == null)
            {
                return false;
            }

            // Note that we can't always rely on the Card instances to stay the same,
            // because any adjustment creates a new instance with slightly different attributes.
            return (card.Id == source.Id) && (game[column.Value][card.Side].Count == Max.CardsPerLocation);
        }
    }
}
