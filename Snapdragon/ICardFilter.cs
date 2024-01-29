namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(Card card);
    }

    public interface ICardFilter<T>
    {
        bool Applies(Card card, T source);
    }
}
