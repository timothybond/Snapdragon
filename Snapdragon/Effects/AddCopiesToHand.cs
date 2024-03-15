using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record AddCopiesToHand(
        ICard Card,
        int Count,
        Func<CardInstance, CardInstance>? Transform = null,
        Side? Side = null
    ) : IEffect
    {
        public Game Apply(Game game)
        {
            var side = Side ?? Card.Side;
            var player = game[side];
            var events = new List<CardAddedToHandEvent>();

            for (var i = 0; i < Count; i++)
            {
                // TODO: Determine if there's any scenarios where this isn't correct
                if (player.Hand.Count >= Max.HandSize)
                {
                    break;
                }

                var card = Card.ToCardInstance() with
                {
                    State = CardState.InHand,
                    Id = Ids.GetNext<ICard>(),
                    Side = side
                };
                if (this.Transform != null)
                {
                    card = this.Transform(card);
                }

                player = player with { Hand = player.Hand.Add(card) };
                events.Add(new CardAddedToHandEvent(card, game.Turn));
            }

            game = game.WithPlayer(player);

            return events.Aggregate(game, (g, e) => g.WithEvent(e));
        }
    }
}
