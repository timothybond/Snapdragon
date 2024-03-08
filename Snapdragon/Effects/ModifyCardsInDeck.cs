using System.Collections.Immutable;

namespace Snapdragon.Effects
{
    public record ModifyCardsInDeck(Side Side, ICardModifier Modifier) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Side];

            player = player with
            {
                Library = player.Library with
                {
                    Cards = player.Library.Cards.Select(Modifier.Apply).ToImmutableList()
                }
            };

            return game.WithPlayer(player);
        }
    }
}
