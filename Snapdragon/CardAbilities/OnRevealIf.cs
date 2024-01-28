using Snapdragon.Events;

namespace Snapdragon.CardAbilities
{
    public record OnRevealIf(ICardCondition Condition, ICardRevealAbility TriggeredAbility)
        : ICardRevealAbility
    {
        public GameState Activate(GameState game, Card source)
        {
            var otherCardsPlayed = game
                .PastEvents.Where(e => e.Turn == game.Turn && e.Type == EventType.CardPlayed)
                .Cast<CardPlayedEvent>();

            if (
                otherCardsPlayed.Any(cpe =>
                    cpe.Card.Side == source.Side.OtherSide() && cpe.Card.Column == source.Column
                )
            )
            {
                return this.TriggeredAbility.Activate(game, source);
            }

            return game;
        }
    }
}
