namespace Snapdragon.Fluent.Filters
{
    public record WithCost(int Cost) : WhereCardFilter<object>
    {
        protected override bool Includes(ICard card, object context)
        {
            return card.Cost == Cost;
        }
    }
}
