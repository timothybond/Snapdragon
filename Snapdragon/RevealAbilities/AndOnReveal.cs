namespace Snapdragon.RevealAbilities
{
    public record AndOnReveal(IRevealAbility<Card> First, IRevealAbility<Card> Second)
        : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            game = First.Activate(game, source);
            game = Second.Activate(game, source);

            return game;
        }
    }
}
