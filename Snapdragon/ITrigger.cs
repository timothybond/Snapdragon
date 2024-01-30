namespace Snapdragon
{
    public interface ITrigger
    {
        bool IsMet(Event e, Game game);
    }
}
