using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record PlayedThisTurn<TContext> : IFilter<ICardInstance, TContext>
    {
        public IEnumerable<ICardInstance> GetFrom(IEnumerable<ICardInstance> initial, TContext context, Game game)
        {
            var cardsPlayedThisTurn = game
                .PastEvents.Concat(game.NewEvents)
                .OfType<CardPlayedEvent>()
                .Where(e => e.Turn == game.Turn)
                .Select(e => e.Card.Id)
                .ToHashSet();

            return initial.Where(c => cardsPlayedThisTurn.Contains(c.Id));
        }
    }
}
