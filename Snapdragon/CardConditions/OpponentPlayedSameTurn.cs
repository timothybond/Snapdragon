using Snapdragon.Events;

namespace Snapdragon.CardConditions
{
    /// <summary>
    /// An instance of <see cref="ICardCondition"/> that is met if the opponent played a <see cref="Card"/> in the same
    /// <see cref="Location"/> on the same turn.
    /// </summary>
    public class OpponentPlayedSameTurn : ICardCondition
    {
        public bool IsMet(Game game, Card source)
        {
            var otherCardsPlayed = game
                .PastEvents.Where(e => e.Turn == game.Turn && e.Type == EventType.CardPlayed)
                .Cast<CardPlayedEvent>();

            return otherCardsPlayed.Any(cpe =>
                cpe.Card.Side == source.Side.OtherSide() && cpe.Card.Column == source.Column
            );
        }
    }
}
