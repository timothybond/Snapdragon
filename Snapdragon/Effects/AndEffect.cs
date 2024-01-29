namespace Snapdragon.Effects
{
    public record AndEffect(IEffect First, IEffect Second) : IEffect
    {
        public GameState Apply(GameState game)
        {
            game = First.Apply(game);
            game = Second.Apply(game);

            return game;
        }
    }
}
