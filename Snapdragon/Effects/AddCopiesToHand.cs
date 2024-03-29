using Snapdragon.Events;
using Snapdragon.Fluent;

namespace Snapdragon.Effects
{
    // TODO: Fix name
    public record AddCopiesToHand(ICardInstance Card, ICardTransform? Transform = null, Side? Side = null)
        : IEffect
    {
        public Game Apply(Game game)
        {
            var side = Side ?? Card.Side;
            var player = game[side];
            var events = new List<CardAddedToHandEvent>();

            // TODO: Determine if there's any scenarios where this isn't correct
            if (player.Hand.Count >= Max.HandSize)
            {
                return game;
            }

            var card = game.GetCard(Card.Id);
            return game.WithCopyInHand(card, Side ?? card.Side, Transform);
        }
    }
}
