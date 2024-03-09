namespace Snapdragon.Effects
{
    public record AddCopiesToHand(ICard Card, int Count, Func<CardInstance, CardInstance>? Transform = null)
        : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Card.Side];

            for (var i = 0; i < Count; i++)
            {
                // TODO: Determine if there's any scenarios where this isn't correct
                if (player.Hand.Count >= 7)
                {
                    break;
                }

                var card = Card.ToCardInstance() with
                {
                    State = CardState.InHand,
                    Id = Ids.GetNext<CardInstance>()
                };
                if (this.Transform != null)
                {
                    card = this.Transform(card);
                }

                player = player with { Hand = player.Hand.Add(card) };
            }

            return game.WithPlayer(player);
        }
    }
}
