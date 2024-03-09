namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(ICard card, Game game);
    }

    public interface ICardFilter<T>
    {
        bool Applies(ICard card, T source, Game game);
    }
}
