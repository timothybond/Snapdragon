namespace Snapdragon
{
    /// <summary>
    /// An event that references a single, specific card.
    /// </summary>
    public interface ICardEvent
    {
        ICard Card { get; }
    }
}
