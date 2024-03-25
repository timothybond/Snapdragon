using Snapdragon.Events;

namespace Snapdragon.Effects
{
    public record DestroyCardInPlay(ICard Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var card = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (card == null)
            {
                return game;
            }

            if (game.GetBlockedEffects(card).Contains(EffectType.DestroyCard))
            {
                return game;
            }

            var location = game[card.Column];
            location = location.WithoutCard(card);

            var player = game[card.Side];
            player = player with
            {
                Destroyed = player.Destroyed.Add(
                    card.ToCardInstance() with
                    {
                        State = CardState.Destroyed
                    }
                )
            };

            return game.WithLocation(location)
                .WithPlayer(player)
                .WithEvent(new CardDestroyedFromPlayEvent(game.Turn, card));
        }
    }
}
