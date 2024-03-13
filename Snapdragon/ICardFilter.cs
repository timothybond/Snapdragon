namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(ICard card, Game game);
    }

    public interface ICardFilter<in T>
    {
        bool Applies(ICard card, T source, Game game);
    }
}
