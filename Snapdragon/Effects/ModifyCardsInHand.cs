using System.Collections.Immutable;

namespace Snapdragon.Effects
{
    public record ModifyCardsInHand(Side Side, ICardModifier Modifier) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Side];

            player = player with { Hand = player.Hand.Select(Modifier.Apply).ToImmutableList() };

            return game.WithPlayer(player);
        }
    }
}
