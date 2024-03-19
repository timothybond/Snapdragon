using Snapdragon.Events;
using Snapdragon.Fluent;

namespace Snapdragon.Effects
{
    /// <summary>
    /// Adds a copy of the given card to a <see cref="Player"/>'s hand, with an optional transform.
    /// </summary>
    /// <param name="Card">The target card to copy.</param>
    /// <param name="Transform">An optional transform.</param>
    /// <param name="Side">The side of the <see cref="Player"/> to give the card to.
    /// If unset, defaults to the side of <see cref="Card"/>.</param>
    public record AddCopyToHand(ICard Card, ICardTransform? Transform = null, Side? Side = null)
        : IEffect
    {
        public Game Apply(Game game)
        {
            var side = Side ?? Card.Side;
            var player = game[side];

            // TODO: Determine if there's any scenarios where this isn't correct
            if (player.Hand.Count >= Max.HandSize)
            {
                return game;
            }

            // TODO: Get the card from wherever it currently is, so the state is up-to-date
            var card = (this.Transform?.Apply(Card) ?? Card).ToCardInstance() with
            {
                State = CardState.InHand,
                Id = Ids.GetNext<ICard>(),
                Side = side
            };

            player = player with { Hand = player.Hand.Add(card) };
            return game.WithPlayer(player).WithEvent(new CardAddedToHandEvent(card, game.Turn));
        }
    }
}
