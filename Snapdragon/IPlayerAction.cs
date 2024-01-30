namespace Snapdragon
{
    /// <summary>
    /// Represents an action that a <see cref="Player"/> can choose to do on a specific Turn.
    /// </summary>
    public interface IPlayerAction
    {
        public Side Side { get; }

        Game Apply(Game game);
    }
}
