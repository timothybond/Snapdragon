namespace Snapdragon.TargetFilters
{
    public class WithOngoingAbility : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            return card.Ongoing != null;
        }
    }
}
