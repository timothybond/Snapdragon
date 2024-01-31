namespace Snapdragon.Effects
{
    /// <summary>
    /// Effect where a player draws a card from their opponent's library.
    /// </summary>
    public record DrawOpponentCard(Side Side) : IEffect
    {
        public Game Apply(Game game)
        {
            var player = game[Side];
            var opponent = game[Side.OtherSide()];

            if (player.Hand.Count < 7 && opponent.Library.Count > 0)
            {
                var card = opponent.Library[0];
                opponent = opponent with { Library = opponent.Library.RemoveAt(0) };

                player = player with { Hand = player.Hand.Add(card with { Side = Side }) };

                return game.WithPlayer(player).WithPlayer(opponent);
            }

            return game;
        }
    }
}
