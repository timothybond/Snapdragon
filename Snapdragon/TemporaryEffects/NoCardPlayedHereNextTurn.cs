using Snapdragon.Events;
using Snapdragon.Triggers;

namespace Snapdragon.TemporaryEffects
{
    public record NoCardPlayedHereNextTurn : ITriggerBuilder<TemporaryEffect<Card>>
    {
        public ITrigger Build(Game game, TemporaryEffect<Card> source)
        {
            var location = source.Column;
            var side = source.Side;
            var nextTurn = game.Turn + 1;

            return new OnTurnEnded().And(
                new ConditionTrigger(game =>
                {
                    if (game.Turn != nextTurn)
                    {
                        return false;
                    }

                    return !game.PastEvents.Any(e =>
                        e is CardRevealedEvent cr
                        && cr.Turn == nextTurn
                        && cr.Card.Side == side
                        && cr.Card.Column == location
                    );
                })
            );
        }
    }
}
