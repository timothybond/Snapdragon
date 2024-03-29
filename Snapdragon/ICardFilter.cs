namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(ICardInstance card, Game game);
    }

    public interface ICardFilter<in T>
    {
        bool Applies(ICardInstance card, T source, Game game);
    }
}
