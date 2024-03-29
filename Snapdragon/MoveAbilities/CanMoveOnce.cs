using Snapdragon.Events;

namespace Snapdragon.MoveAbilities
{
    /// <summary>
    /// Represents a card that can move exactly once (e.g. Nightcrawler).
    /// </summary>
    public record CanMoveOnce : IMoveAbility<ICard>
    {
        public bool CanMove(
            ICard target,
            ICard source,
            Column destination,
            Game game
        )
        {
            // Note - from testing this is consistent with the way Nightcrawler works:
            // even if something else (e.g. Cloak) allowed him to move, it still
            // seems to use up the once-per-game move ability
            return target.Id == source.Id
                && !game.PastEvents.Any(e =>
                    e is CardMovedEvent cardMoved && cardMoved.Card.Id == source.Id
                );
        }
    }
}
