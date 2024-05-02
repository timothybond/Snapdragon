namespace Snapdragon.Fluent.Filters
{
    public record WithOngoingAbilities : WhereCardFilter<object>
    {
        public override bool Applies(ICardInstance card, object context, Game game)
        {
            return card.Ongoing != null;
        }
    }
}
