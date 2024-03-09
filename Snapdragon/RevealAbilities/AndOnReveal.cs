namespace Snapdragon.RevealAbilities
{
    public record AndOnReveal<T>(IRevealAbility<T> First, IRevealAbility<T> Second)
        : IRevealAbility<T>
    {
        public Game Activate(Game game, T source)
        {
            game = First.Activate(game, source);
            game = Second.Activate(game, source);

            return game;
        }
    }
}
