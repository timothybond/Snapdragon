namespace Snapdragon
{
    public interface ICardInPlay : ICard
    {
        new Column Column { get; }
    }
}
