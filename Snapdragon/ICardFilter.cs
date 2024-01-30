namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(Card card, Game game);
    }

    public interface ICardFilter<T>
    {
        bool Applies(Card card, T source, Game game);
    }
}
