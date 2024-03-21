namespace Snapdragon
{
    /// <summary>
    /// An object that will be revealed at some specific turn.
    /// </summary>
    public interface IRevealableObject
    {
        int? TurnRevealed { get; }
    }
}
