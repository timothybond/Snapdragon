using Snapdragon.Events;

namespace Snapdragon.Effects
{
    /// <summary>
    /// Adds a specific card to the given player's hand.
    /// </summary>
    public record AddCardToHand(CardDefinition Definition, Side Side) : IEffect
    {
        public Game Apply(Game game)
        {
            // Note we normally enforce hand-size limit elsewhere.
            // TODO: find a way to centralize this
            if (game[Side].Hand.Count >= Max.HandSize)
            {
                return game;
            }

            var card = new CardInstance(Definition, Side, CardState.InHand);

            return game.WithPlayer(game[Side] with { Hand = game[Side].Hand.Add(card) })
                .WithEvent(new CardAddedToHandEvent(card, game.Turn));
        }
    }
}
