namespace Snapdragon
{
    public interface ICardRevealAbility : ICardAbility
    {
        GameState Activate(GameState game, Card source);
    }
}
