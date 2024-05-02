namespace Snapdragon.Fluent.Selectors
{
    public class SnapCardsSelector : ISelector<CardDefinition, object>
    {
        public IEnumerable<CardDefinition> Get(object context, Game game)
        {
            return SnapCards.All;
        }

        public bool Selects(CardDefinition item, object context, Game game)
        {
            // TODO: See if there's a good alternative to this (although it likely doesn't matter much)
            return SnapCards.All.Contains(item);
        }
    }

    public class SnapCardsSelector<TContext> : ISelector<CardDefinition, TContext>
    {
        public IEnumerable<CardDefinition> Get(TContext context, Game game)
        {
            return SnapCards.All;
        }

        public bool Selects(CardDefinition item, TContext context, Game game)
        {
            // TODO: See if there's a good alternative to this (although it likely doesn't matter much)
            return SnapCards.All.Contains(item);
        }
    }
}
