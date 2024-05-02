namespace Snapdragon.Fluent.Filters
{
    public record WithCost(int Cost) : WhereCardFilter<object>, IFilter<CardDefinition, object>
    {
        public IEnumerable<CardDefinition> GetFrom(
            IEnumerable<CardDefinition> initial,
            object context,
            Game game
        )
        {
            return initial.Where(cd => cd.Cost == Cost);
        }

        public override bool Applies(ICardInstance card, object context, Game game)
        {
            return card.Cost == Cost;
        }

        public bool Applies(CardDefinition item, object context, Game game)
        {
            return item.Cost == Cost;
        }
    }
}
