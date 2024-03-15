namespace Snapdragon.Effects
{
    /// <summary>
    /// Effect where the given Player draws a card.
    /// </summary>
    public record DrawCard(Side Side) : IEffect
    {
        public Game Apply(Game game)
        {
            return game.DrawCard(Side);
        }
    }
}
