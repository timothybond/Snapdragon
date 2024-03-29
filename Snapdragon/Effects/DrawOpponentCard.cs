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
            var opponent = game[Side.Other()];

            if (player.Hand.Count < Max.HandSize && opponent.Library.Count > 0)
            {
                return game.DrawOpponentCard(Side);
            }

            return game;
        }
    }
}
