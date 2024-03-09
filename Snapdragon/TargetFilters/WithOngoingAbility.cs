namespace Snapdragon.TargetFilters
{
    public class WithOngoingAbility : ICardFilter<ICard>
    {
        public bool Applies(ICard card, ICard source, Game game)
        {
            return card.Ongoing != null;
        }
    }
}
