namespace Snapdragon.TargetFilters
{
    public class WithOngoingAbility : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, GameState game)
        {
            return card.Ongoing != null;
        }
    }
}
