namespace Snapdragon
{
    public interface ITrigger
    {
        bool IsMet(Event e, GameState game);
    }
}
