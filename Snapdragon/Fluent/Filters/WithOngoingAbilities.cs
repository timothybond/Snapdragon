namespace Snapdragon.Fluent.Filters
{
    public record WithOngoingAbilities : WhereCardFilter<object>
    {
        protected override bool Includes(ICardInstance card, object context)
        {
            return card.Ongoing != null;
        }
    }
}
