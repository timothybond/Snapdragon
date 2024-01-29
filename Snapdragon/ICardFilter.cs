namespace Snapdragon
{
    public interface ICardFilter
    {
        bool Applies(Card card, Card source);
    }
}
