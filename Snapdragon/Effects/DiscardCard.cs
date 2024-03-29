namespace Snapdragon.Effects
{
    public record DiscardCard(ICardInstance card) : IEffect
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

            return game.WithCardDiscarded(cardInHand);
        }
    }
}
