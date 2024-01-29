namespace Snapdragon
{
    public interface IEffect
    {
        GameState Apply(GameState game);
    }
}
