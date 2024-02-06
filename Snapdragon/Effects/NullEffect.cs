namespace Snapdragon.Effects
{
    public record NullEffect() : IEffect
    {
        public Game Apply(Game game)
        {
            return game;
        }
    }
}
