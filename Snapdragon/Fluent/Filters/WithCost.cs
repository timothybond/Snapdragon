namespace Snapdragon.Fluent.Filters
{
    public record WithCost(int Cost) : WhereCardFilter<object>, IFilter<CardDefinition, object>
    {
        public IEnumerable<CardDefinition> GetFrom(
            IEnumerable<CardDefinition> initial,
            object context
, Game game)
        {
            return initial.Where(cd => cd.Cost == Cost);
        }

        protected override bool Includes(ICardInstance card, object context)
        {
            return card.Cost == Cost;
        }
    }
}
