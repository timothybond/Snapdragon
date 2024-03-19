using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record DiscardCard(ICard card) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[card.Side];
            var hand = player.Hand;
            var cardInHand = hand.FirstOrDefault(c => c.Id == card.Id);

            if (cardInHand == null)
            {
                return game;
            }

            var discardedCard = cardInHand with { State = CardState.Discarded };

            player = game[card.Side] with
            {
                Hand = hand.Remove(cardInHand),
                Discards = player.Discards.Add(discardedCard)
            };

            return game.WithPlayer(player)
                .WithEvent(new CardDiscardedEvent(game.Turn, discardedCard));
        }
    }
}
