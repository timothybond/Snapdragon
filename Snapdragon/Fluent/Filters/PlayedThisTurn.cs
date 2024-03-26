using Snapdragon.Events;

namespace Snapdragon.Fluent.Filters
{
    public record PlayedThisTurn<TContext> : IFilter<ICard, TContext>
    {
        public IEnumerable<ICard> GetFrom(IEnumerable<ICard> initial, TContext context, Game game)
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
