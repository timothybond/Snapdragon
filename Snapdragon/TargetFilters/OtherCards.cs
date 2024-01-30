namespace Snapdragon.TargetFilters
{
    public record OtherCards : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            // Note that we can't always rely on the Card instances to stay the same,
            // because any adjustment creates a new instance with slightly different attributes.
            return (card.Id != source.Id);
        }
    }
}
