namespace Snapdragon.TargetFilters
{
    public class WithOngoingAbility : ICardFilter<ICardInstance>
    {
        public bool Applies(ICardInstance card, ICardInstance source, Game game)
        {
            return card.Ongoing != null;
        }
    }
}
