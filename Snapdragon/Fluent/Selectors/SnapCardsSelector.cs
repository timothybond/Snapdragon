namespace Snapdragon.Fluent.Selectors
{
    public class SnapCardsSelector : ISelector<CardDefinition, object>
    {
        public IEnumerable<CardDefinition> Get(object context, Game game)
        {
            return SnapCards.All;
        }
    }

    public class SnapCardsSelector<TContext> : ISelector<CardDefinition, TContext>
    {
        public IEnumerable<CardDefinition> Get(TContext context, Game game)
        {
            return SnapCards.All;
        }
    }
}
